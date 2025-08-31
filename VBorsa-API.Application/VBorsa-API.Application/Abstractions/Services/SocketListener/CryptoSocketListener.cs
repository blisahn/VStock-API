using MediatR;
using VBorsa_API.Application.Abstractions.Services.Redis;
using VBorsa_API.Application.Abstractions.Services.Symbol;
using VBorsa_API.Application.DTOs.Quote;
using VBorsa_API.Application.DTOs.Quote.Binance;
using VBorsa_API.Application.Features.Notifications;
using VBorsa_API.Application.Features.Notifications.Binance;
using VBorsa_API.Application.Repositories.Socket;

namespace VBorsa_API.Application.Abstractions.Services.SocketListener;

public class CryptoSocketListener : ICryptoSocketListener
{
    private readonly IMediator _mediator;
    private readonly ISymbolService _symbolService;
    private readonly IRedisService _redisService;

    public CryptoSocketListener(IMediator mediator, ISymbolService symbolService, IRedisService redisService)
    {
        _mediator = mediator;
        _symbolService = symbolService;
        _redisService = redisService;
    }

    public async Task ProcessFinHubTradeData(FinhubTradeDataDto finhubTradeData)
    {
        await _mediator.Publish(new FinHubCryptoDataUpdateNotification { Data = finhubTradeData });
    }

    public async Task ProcessKlineData(KlineDataDto klineData)
    {
        await _mediator.Publish(new BinanceKlineDataUpdateNotification
        {
            Data = klineData
        });
    }

    public async Task ProcessDepthData(OrderBookDto orderBookDto)
    {
        await _mediator.Publish(new BinanceDepthDataUpdateNotification
        {
            Data = orderBookDto
        });
    }

    public async Task ProcessTradeData(TradeDto tradeData)
    {
        await _redisService.AddAsync(tradeData.Price,
            new DateTimeOffset(tradeData.RetrievedAt).ToUnixTimeSeconds(), tradeData.Symbol, "BINANCE",
            "BINANCE");

        var publiclyVisibleSymbols = await GetPubliclyVisibleSymbolsFromCacheAsync();
        if (publiclyVisibleSymbols.Contains(tradeData.Symbol))
            await _mediator.Publish(new BinancePublicMarketDataUpdateNotification { Data = tradeData });

        await _mediator.Publish(new BinanceTradeDataUpdateNotification { Data = tradeData });
    }

    private async Task<HashSet<string>> GetPubliclyVisibleSymbolsFromCacheAsync()
    {
        const string publicSymbolsCacheKey = "public_symbols";
        var cachedSymbols = await _redisService.GetAsync<HashSet<string>>(publicSymbolsCacheKey);
        if (cachedSymbols != null) return cachedSymbols;
        var symbolsFromService = await _symbolService.GetPubliclyVisibleSymbolsAsync();
        var symbolSet = new HashSet<string>(symbolsFromService);
        await _redisService.SetAsync(publicSymbolsCacheKey, symbolSet, TimeSpan.FromMinutes(5));
        return symbolSet;
    }
}