using MediatR;
using Microsoft.AspNetCore.SignalR;
using VBorsa_API.Application.Features.Notifications.Binance;
using VBorsa_API.Infrastructure.Helpers;
using VBorsa_API.Presentation.Hubs;
using VBorsa_API.Presentation.Hubs.Crypto;

namespace VBorsa_API.Presentation.Handlers.CryptoNotification.Binance;

public class BinanceTradeDataUpdateNotificationHandler : INotificationHandler<BinanceTradeDataUpdateNotification>
{
    private readonly IHubContext<PrivateCryptoHub> _privateHubContext;
 
    public BinanceTradeDataUpdateNotificationHandler(IHubContext<PrivateCryptoHub> privateHubContext)
    {
        _privateHubContext = privateHubContext;
    }

    public async Task Handle(BinanceTradeDataUpdateNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Data is null) return;
        var symbol = notification.Data.Symbol;
        await _privateHubContext.Clients.Group(Groups.Trade(symbol))
            .SendAsync("ReceiveBinanceTradeDataUpdateNotification", notification.Data, cancellationToken);
        await _privateHubContext.Clients.Group(Groups.MarketAllTrades)
            .SendAsync("ReceiveBinanceMarketData", notification.Data, cancellationToken);
    }
}