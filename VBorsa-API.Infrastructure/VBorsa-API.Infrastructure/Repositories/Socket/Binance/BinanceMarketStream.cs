using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VBorsa_API.Application.Abstractions.Services.Symbol;
using VBorsa_API.Application.Exceptions;

namespace VBorsa_API.Infrastructure.Repositories.Socket.Binance;

public class BinanceMarketStream : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BinanceStreamManager _streamManager;
    private readonly TimeSpan _resyncInterval = TimeSpan.FromSeconds(15);

    public BinanceMarketStream(IServiceScopeFactory scopeFactory, BinanceStreamManager streamManager)
    {
        _scopeFactory = scopeFactory;
        _streamManager = streamManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SyncActiveSymbols(stoppingToken);
            await Task.Delay(_resyncInterval, stoppingToken);
        }
    }

    private async Task SyncActiveSymbols(CancellationToken stoppingToken)
    {
        try
        {
            HashSet<string> activeDbSymbols = new HashSet<string>();

            using (var scope = _scopeFactory.CreateScope())
            {
                var symbolService = scope.ServiceProvider.GetRequiredService<ISymbolService>();
                var symbols = await symbolService.GetActiveSymbolsAsync();
                activeDbSymbols = new HashSet<string>(symbols.Select(s => s.ToLowerInvariant()),
                    StringComparer.OrdinalIgnoreCase);
            }

            var currentSubscriptions = _streamManager.GetCurrentSubscriptions();
            var currentSymbols = new HashSet<string>(
                currentSubscriptions.Select(sub => sub.Split('@')[0]),
                StringComparer.OrdinalIgnoreCase
            );

            var symbolsToSubscribe = activeDbSymbols.Except(currentSymbols).ToList();
            var symbolsToUnsubscribe = currentSymbols.Except(activeDbSymbols).ToList();

            if (symbolsToSubscribe.Any())
            {
                Console.WriteLine($"Yeni abonelikler ekleniyor: {string.Join(", ", symbolsToSubscribe)}");
                foreach (var symbol in symbolsToSubscribe)
                {
                    var streamKey = $"{symbol}@aggTrade";
                    await _streamManager.SubscribeToStreamAsync(streamKey, stoppingToken);
                }
            }

            if (symbolsToUnsubscribe.Any())
            {
                Console.WriteLine($"Abonelikler kaldırılıyor: {string.Join(", ", symbolsToUnsubscribe)}");
                foreach (var symbol in symbolsToUnsubscribe)
                {
                    var streamKeyTrade = $"{symbol}@aggTrade";
                    await _streamManager.DecrementAndUnsubscribeIfNecessaryAsync(streamKeyTrade);
                }
            }

            if (!activeDbSymbols.Any())
            {
                Console.WriteLine(
                    "Sistemde takip edilecek aktif bir varlık bulunmuyor. 15 saniye sonra tekrar denenecek.");
            }
        }
        catch (Exception ex)
        {
            throw new MarketListFailedException(
                "Degerli varliklar listelenirken beklenmeyen bir hata ile karsilasildi");
        }
    }
}