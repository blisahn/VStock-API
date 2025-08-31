using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using VBorsa_API.Application.DTOs.Quote;
using VBorsa_API.Application.DTOs.Socket;
using VBorsa_API.Application.Repositories.Socket;

namespace VBorsa_API.Infrastructure.Repositories.Socket.FinHub;

public class FinHubCryptoSocket : BackgroundService
{
    private readonly ClientWebSocket _ws = new();
    private readonly Uri _uri;
    private readonly IConfiguration _config;
    private readonly ICryptoSocketListener _socketListener;

    public FinHubCryptoSocket(IConfiguration config, ICryptoSocketListener socketListener)
    {
        _config = config;
        _uri = new Uri($"{_config["FinHubCryptoSocket:Uri"]}{_config["FinHubCryptoSocket:ApiKey"]}");
        _socketListener = socketListener;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _ws.ConnectAsync(_uri, stoppingToken);
        await SubscribeToSymbol("BINANCE:BTCUSDT", stoppingToken);
        await SubscribeToSymbol("BINANCE:ETHUSDT", stoppingToken);
        await SubscribeToSymbol("BINANCE:AVAXUSDT", stoppingToken);
        await SubscribeToSymbol("BINANCE:SOLUSDT", stoppingToken);
        await SubscribeToSymbol("AAPL", stoppingToken);
        await SubscribeToSymbol("AMZN", stoppingToken);
        await SubscribeToSymbol("MSFT", stoppingToken);
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var buffer = new byte[1024 * 4];
                var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                if (string.IsNullOrEmpty(message) || message.Contains("ping"))
                    continue;

                var finHubMessage = JsonSerializer.Deserialize<FinHubSocketMessage>(message);
                if (finHubMessage!.Type == "trade")
                {
                    // Console.WriteLine("Token guncellenecek");

                    foreach (var tradeData in finHubMessage.Data)
                    {
                        FinhubTradeDataDto finhubTradeDataDto;
                        if (tradeData.Symbol.Contains(':'))
                        {
                            var sourceSymbol = tradeData.Symbol.Split(':');
                            finhubTradeDataDto = new FinhubTradeDataDto()
                            {
                                Symbol = sourceSymbol[1],
                                Price = tradeData.Price,
                                RetrievedAt = tradeData.Timestamp,
                                Source = sourceSymbol[0],
                                Provider = "FinHub",
                            };
                        }
                        else
                        {
                            var sourceSymbol = tradeData.Symbol;
                            finhubTradeDataDto = new FinhubTradeDataDto()
                            {
                                Symbol = sourceSymbol,
                                Price = tradeData.Price,
                                RetrievedAt = tradeData.Timestamp,
                                Source = "SP",
                                Provider = "FinHub",
                            };
                        }

                        await _socketListener.ProcessFinHubTradeData(finhubTradeDataDto);
                    }
                }
                else
                {
                    Console.WriteLine("Something went wrong!");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception Occured: {e.Message}");
        }
    }

    private async Task SubscribeToSymbol(string symbol, CancellationToken stoppingToken)
    {
        var subscribeMessage = JsonSerializer.Serialize(new { @type = "subscribe", symbol });
        var bytes = Encoding.UTF8.GetBytes(subscribeMessage);
        await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, stoppingToken);
    }
}