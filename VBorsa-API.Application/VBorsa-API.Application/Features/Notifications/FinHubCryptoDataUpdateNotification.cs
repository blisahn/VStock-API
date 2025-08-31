using MediatR;
using VBorsa_API.Application.DTOs.Quote;

namespace VBorsa_API.Application.Features.Notifications;

public class FinHubCryptoDataUpdateNotification : INotification
{
    public FinhubTradeDataDto? Data { get; set; }
}