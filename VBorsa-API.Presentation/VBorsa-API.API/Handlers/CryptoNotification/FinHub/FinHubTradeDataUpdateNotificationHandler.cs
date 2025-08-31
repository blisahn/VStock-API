using MediatR;
using Microsoft.AspNetCore.SignalR;
using VBorsa_API.Application.Features.Notifications;
using VBorsa_API.Presentation.Hubs;
using VBorsa_API.Presentation.Hubs.Crypto;

namespace VBorsa_API.Presentation.Handlers.CryptoNotification.FinHub;

public class FinHubTradeDataUpdateNotificationHandler : INotificationHandler<FinHubCryptoDataUpdateNotification>
{
    private readonly IHubContext<PrivateCryptoHub> _hubContext;

    public FinHubTradeDataUpdateNotificationHandler(IHubContext<PrivateCryptoHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Handle(FinHubCryptoDataUpdateNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Data != null)
            await _hubContext.Clients.All.SendAsync("ReceiveFinHubCryptoDataUpdateNotification", notification.Data,
                cancellationToken);
    }
}