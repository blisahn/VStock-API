using System.Collections.Concurrent;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VBorsa_API.Application.Abstractions.Services.Symbol;
using VBorsa_API.Application.DTOs.Quote.Binance;
using VBorsa_API.Application.DTOs.Socket;
using VBorsa_API.Application.Repositories.Socket;

namespace VBorsa_API.Infrastructure.Repositories.Socket.Binance;

public class BinanceStreamManager
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;
    private readonly string _baseStreamUri;
    private readonly ConcurrentDictionary<string, ClientWebSocket> _activeSockets = new();
    private readonly ConcurrentDictionary<string, int> _subscriberCounts = new();
    // private readonly ISymbolService _symbolService;

    public BinanceStreamManager(IConfiguration config, IServiceScopeFactory scopeFactory)
    {
        _config = config;
        _scopeFactory = scopeFactory;
        _baseStreamUri = _config["BinanceCryptoSocket:StreamUri"] ?? "wss://stream.binance.com:9443/stream?streams=";
    }

    public async Task SubscribeToStreamAsync(string streamKey, CancellationToken stoppingToken)
    {
        _subscriberCounts.AddOrUpdate(streamKey, 1, (key, count) => count + 1);
        if (_activeSockets.ContainsKey(streamKey))
        {
            Console.WriteLine(
                $"Stream zaten aktif, abone sayısı güncellendi: {streamKey} -> {_subscriberCounts[streamKey]}");
            return;
        }

        var fullUri = new Uri($"{_baseStreamUri}{streamKey}");
        var ws = new ClientWebSocket();
        _activeSockets.TryAdd(streamKey, ws);
        try
        {
            await ws.ConnectAsync(fullUri, stoppingToken);
            Console.WriteLine($"Yeni stream bağlandı: {streamKey}");
            _ = Task.Run(async () => await ListenForDataAsync(ws, streamKey, stoppingToken), stoppingToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Stream bağlantı hatası ({streamKey}): {ex.Message}");
            _activeSockets.TryRemove(streamKey, out _);
            ws.Dispose();
        }
    }

    public async Task DecrementAndUnsubscribeIfNecessaryAsync(string streamKey)
    {
        if (!_subscriberCounts.ContainsKey(streamKey)) return;

        var newCount = _subscriberCounts.AddOrUpdate(streamKey, 0, (key, count) => count - 1);

        Console.WriteLine($"Stream'den abone ayrıldı: {streamKey} -> Kalan: {newCount}");
        if (newCount <= 0)
        {
            _subscriberCounts.TryRemove(streamKey, out _);
            await UnsubscribeFromStreamAsync(streamKey);
        }
    }

    private async Task UnsubscribeFromStreamAsync(string streamKey)
    {
        if (_activeSockets.TryRemove(streamKey, out var ws))
        {
            if (ws.State == WebSocketState.Open)
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Unsubscribing", CancellationToken.None);
            }

            ws.Dispose();
            Console.WriteLine($"Stream bağlantısı kesildi: {streamKey}");
        }
    }

    private async Task ListenForDataAsync(ClientWebSocket ws, string streamKey, CancellationToken stoppingToken)
    {
        var buffer = new byte[1024 * 4];
        while (ws.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                if (string.IsNullOrEmpty(message) || message.Contains("ping")) continue;

                var doc = JsonDocument.Parse(message);
                using var scope = _scopeFactory.CreateScope();
                var listener = scope.ServiceProvider.GetRequiredService<ICryptoSocketListener>();
                var symbol = streamKey!.Split('@')[0].ToUpper();
                if (streamKey.Contains("@kline"))
                {
                    var klineMessage = JsonSerializer.Deserialize<BinanceSocketMessage<KlineData>>(message);
                    KlineInfo binanceKlineInfo = klineMessage!.Data.KlineInfo;
                    var interval = streamKey.Split('_').Last();
                    if (!binanceKlineInfo.IsClosed) continue;
                    var klineDto = new KlineDataDto
                    {
                        Interval = interval,
                        Symbol = symbol,
                        OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(binanceKlineInfo!.StartTime)
                            .UtcDateTime,
                        CloseTime = DateTimeOffset.FromUnixTimeMilliseconds(binanceKlineInfo.CloseTime)
                            .UtcDateTime,
                        OpenPrice = decimal.Parse(binanceKlineInfo.OpenPrice, CultureInfo.InvariantCulture),
                        HighPrice = decimal.Parse(binanceKlineInfo.HighPrice, CultureInfo.InvariantCulture),
                        LowPrice = decimal.Parse(binanceKlineInfo.LowPrice, CultureInfo.InvariantCulture),
                        ClosePrice = decimal.Parse(binanceKlineInfo.ClosePrice, CultureInfo.InvariantCulture),
                        Volume = decimal.Parse(binanceKlineInfo.Volume, CultureInfo.InvariantCulture),
                        QuoteAssetVolume =
                            decimal.Parse(binanceKlineInfo.QuoteAssetVolume, CultureInfo.InvariantCulture),
                        NumberOfTrades = binanceKlineInfo.NumberOfTrades,
                        IsClosed = binanceKlineInfo.IsClosed,
                    };
                    await listener.ProcessKlineData(klineDto);
                }
                else if (streamKey.Contains("@aggTrade"))
                {
                    var tradeMessage = JsonSerializer.Deserialize<BinanceSocketMessage<AggregateTradeData>>(message);
                    var tradeDto = new TradeDto
                    {
                        Symbol = symbol,
                        Price = decimal.Parse(tradeMessage.Data.Price, CultureInfo.InvariantCulture),
                        Quantity = decimal.Parse(tradeMessage.Data.Quantity, CultureInfo.InvariantCulture),
                        RetrievedAt = DateTimeOffset.FromUnixTimeMilliseconds(tradeMessage.Data.Timestamp).UtcDateTime,
                        Side = tradeMessage.Data.IsBuyerMarketMaker ? TradeSide.Sell : TradeSide.Buy
                    };

                    await listener.ProcessTradeData(tradeDto);
                }
                else if (streamKey.Contains("@depth"))
                {
                    var depthMessage = JsonSerializer.Deserialize<BinanceSocketMessage<DepthData>>(message);
                    var orderBookDto = new OrderBookDto()
                    {
                        Symbol = symbol,
                        Bids = depthMessage!.Data.Bids.Select(b => new OrderBookLevelDto
                        {
                            Price = decimal.Parse(b[0], CultureInfo.InvariantCulture),
                            Quantity = decimal.Parse(b[1], CultureInfo.InvariantCulture)
                        }).ToList(),
                        Asks = depthMessage!.Data.Asks.Select(a => new OrderBookLevelDto
                        {
                            Price = decimal.Parse(a[0], CultureInfo.InvariantCulture),
                            Quantity = decimal.Parse(a[1], CultureInfo.InvariantCulture)
                        }).ToList()
                    };
                    await listener.ProcessDepthData(orderBookDto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Stream dinleme hatası ({streamKey}): {ex.Message}");
                break;
            }
        }

        await DecrementAndUnsubscribeIfNecessaryAsync(streamKey);
    }

    public ICollection<string> GetCurrentSubscriptions()
    {
        return _activeSockets.Keys;
    }
}