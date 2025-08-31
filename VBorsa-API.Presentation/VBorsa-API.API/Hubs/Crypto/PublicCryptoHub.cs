using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using VBorsa_API.Application.Abstractions.Services.Symbol;
using VBorsa_API.Infrastructure.Repositories.Socket.Binance;

namespace VBorsa_API.Presentation.Hubs;

public class PublicCryptoHub : Hub
{
    private readonly BinanceStreamManager _streamManager;
    private readonly ISymbolService _symbolService;
    private static int _marketTradesSubscribers = 0;
    public PublicCryptoHub(BinanceStreamManager streamManager, ISymbolService symbolService)
    {
        _streamManager = streamManager;
        _symbolService = symbolService;
    }


    public async Task Join(string group)
    {
        if (group != Infrastructure.Helpers.Groups.MarketAllTrades)
            throw new HubException("Public hub sadece genel market akışını destekler.");
        
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        if (Interlocked.Increment(ref _marketTradesSubscribers) >= 1)
        {
            var symbols = await _symbolService.GetPubliclyVisibleSymbolsAsync();
            foreach (var s in symbols)
                await _streamManager.SubscribeToStreamAsync($"{s.ToLowerInvariant()}@aggTrade", Context.ConnectionAborted);
        }
    }

    public async Task Leave(string group)
    {
        if (group != Infrastructure.Helpers.Groups.MarketAllTrades) return;
        if (Interlocked.Decrement(ref _marketTradesSubscribers) == 0)
        {
            var subs = _streamManager.GetCurrentSubscriptions()
                .Where(x => x.EndsWith("@aggTrade", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var key in subs)
                await _streamManager.DecrementAndUnsubscribeIfNecessaryAsync(key);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}