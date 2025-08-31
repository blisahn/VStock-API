using MediatR;
using Microsoft.AspNetCore.SignalR;
using VBorsa_API.Application.Features.Notifications.Binance;
using VBorsa_API.Infrastructure.Helpers;
using VBorsa_API.Presentation.Hubs;

namespace VBorsa_API.Presentation.Handlers.CryptoNotification.Binance;

public class BinancePublicMarketDataUpdateNotificationHandler : INotificationHandler<BinancePublicMarketDataUpdateNotification>
{
    private readonly IHubContext<PublicCryptoHub> _publicCryptoHub;

    public BinancePublicMarketDataUpdateNotificationHandler(IHubContext<PublicCryptoHub> publicCryptoHub)
    {
        _publicCryptoHub = publicCryptoHub;
    }

    public async Task Handle(BinancePublicMarketDataUpdateNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Data is null) return;
        await _publicCryptoHub.Clients.Group(Groups.MarketAllTrades)
            .SendAsync("ReceivePublicBinanceMarketData", notification.Data, cancellationToken);
    }
}