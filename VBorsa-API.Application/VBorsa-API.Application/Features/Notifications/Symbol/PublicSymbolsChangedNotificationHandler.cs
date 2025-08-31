using MediatR;
using VBorsa_API.Application.Abstractions.Services.Redis;

namespace VBorsa_API.Application.Features.Notifications.Symbol;

public class PublicSymbolsChangedNotificationHandler : INotificationHandler<PublicSymbolsChangedNotification>
{
    private readonly IRedisService _redisService;

    public PublicSymbolsChangedNotificationHandler(IRedisService redisService)
    {
        _redisService = redisService;
    }

    private const string PublicSymbolsCacheKey = "public_symbols";
    public async Task Handle(PublicSymbolsChangedNotification notification, CancellationToken cancellationToken)
    {
        await _redisService.DeleteAsync(PublicSymbolsCacheKey);
        
    }
}