using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using VBorsa_API.Application.Abstractions.Services.Redis;
using VBorsa_API.Application.Abstractions.Services.SocketListener;
using VBorsa_API.Application.Repositories.Socket;
using VBorsa_API.Infrastructure.Helpers;
using VBorsa_API.Infrastructure.Repositories.Redis;
using VBorsa_API.Infrastructure.Repositories.Socket;
using VBorsa_API.Infrastructure.Repositories.Socket.Binance;
using VBorsa_API.Infrastructure.Repositories.Socket.FinHub;

namespace VBorsa_API.Infrastructure;

public static class ServiceRegistration
{
    public static void AddInfraStructureServices(this IServiceCollection services)
    {
        services.AddScoped<ICryptoSocketListener, CryptoSocketListener>();
        services.AddHostedService<BinanceMarketStream>();
        services.AddSingleton<ConnectionMultiplexer>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var redisConnectionString = config.GetConnectionString("VStockCache");
            return ConnectionMultiplexer.Connect(redisConnectionString!);
        });
        services.AddSingleton<IRedisService, RedisService>();
    }
   
    

    public static void AddCryptoBackgroundService(this IServiceCollection services, SocketProviders provider)
    {
        switch (provider)
        {
            case SocketProviders.FINHUB:
                services.AddHostedService<FinHubCryptoSocket>();
                break;
            case SocketProviders.BINANCE:
            default:
                services.AddSingleton<BinanceStreamManager>();
                break;
        }
    }
}