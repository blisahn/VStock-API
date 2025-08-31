using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VBorsa_API.Application.Features.Notifications.Binance;
using VBorsa_API.Infrastructure.Helpers;
using VBorsa_API.Presentation.Hubs;
using VBorsa_API.Presentation.Hubs.Crypto;

namespace VBorsa_API.Presentation.Handlers.CryptoNotification.Binance;

[Authorize]
public class BinanceKlineDataUpdateNotificationHandler : INotificationHandler<BinanceKlineDataUpdateNotification>
{
    private readonly IHubContext<PrivateCryptoHub> _hubContext;

    public BinanceKlineDataUpdateNotificationHandler(IHubContext<PrivateCryptoHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Handle(BinanceKlineDataUpdateNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Data is null)
            return;

        var symbol = notification.Data.Symbol;
        var interval = notification.Data.Interval;
        await _hubContext
            .Clients.Group(Groups.Kline(symbol, interval))
            .SendAsync("ReceiveBinanceKlineDataUpdateNotification", notification.Data, cancellationToken);
    }
}