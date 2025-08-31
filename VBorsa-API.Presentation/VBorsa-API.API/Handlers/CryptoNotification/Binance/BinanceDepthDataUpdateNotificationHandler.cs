using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VBorsa_API.Application.Features.Notifications;
using VBorsa_API.Application.Features.Notifications.Binance;
using VBorsa_API.Infrastructure.Helpers;
using VBorsa_API.Presentation.Hubs;
using VBorsa_API.Presentation.Hubs.Crypto;

namespace VBorsa_API.Presentation.Handlers.CryptoNotification.Binance;

[Authorize]
public class BinanceDepthDataUpdateNotificationHandler : INotificationHandler<BinanceDepthDataUpdateNotification>
{
    private readonly IHubContext<PrivateCryptoHub> _hubContext;

    public BinanceDepthDataUpdateNotificationHandler(IHubContext<PrivateCryptoHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Handle(BinanceDepthDataUpdateNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Data is null) return;
        await _hubContext.Clients.Group(Groups.Depth(notification.Data.Symbol))
            .SendAsync("ReceiveBinanceDepthDataUpdateNotification", notification.Data, cancellationToken);
    }
}