using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VBorsa_API.Infrastructure.Repositories.Socket.Binance;

// Groups sınıfı için

namespace VBorsa_API.Presentation.Hubs.Crypto;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PrivateCryptoHub : Hub
{
    private readonly BinanceStreamManager _streamManager;

    public PrivateCryptoHub(BinanceStreamManager streamManager)
    {
        _streamManager = streamManager;
    }

    private string GetBinanceStreamKey(string group)
    {
        var parts = group.Split('_');
        if (parts.Length < 2)
        {
            throw new ArgumentException($"Geçersiz grup formatı: {group}");
        }

        var symbol = parts[1].ToLowerInvariant();
        var streamType = parts[0].ToLowerInvariant();

        switch (streamType)
        {
            case "kline":
                if (parts.Length < 3)
                    throw new ArgumentException("K-line grubu için aralık (interval) belirtilmelidir.");
                var interval = parts[2].ToLowerInvariant();
                return $"{symbol}@kline_{interval}";
            case "trade":
                return $"{symbol}@aggTrade";
            case "depth":
                var limit = parts.Length > 2 ? parts[2] : "5";
                return $"{symbol}@depth{limit}@100ms";
            default:
                throw new ArgumentException("Bilinmeyen grup tipi.");
        }
    }

    public async Task Join(string group)
    {
        if (!Context.Items.TryGetValue("UserGroups", out var userGroupsObject))
        {
            userGroupsObject = new ConcurrentBag<string>();
            Context.Items["UserGroups"] = userGroupsObject;
        }

        var userGroups = (ConcurrentBag<string>)userGroupsObject;

        userGroups.Add(group);
        await Groups.AddToGroupAsync(Context.ConnectionId, group);

        if (group == Infrastructure.Helpers.Groups.MarketAllTrades)
        {
            Console.WriteLine($"Kullanıcı mantıksal gruba katıldı: {group}");
            return;
        }

        try
        {
            var streamKey = GetBinanceStreamKey(group);
            await _streamManager.SubscribeToStreamAsync(streamKey, Context.ConnectionAborted);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Geçersiz grup adı: {group} -> {ex.Message}");
        }
    }

    public async Task Leave(string group)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);

        if (group == Infrastructure.Helpers.Groups.MarketAllTrades)
        {
            Console.WriteLine($"Kullanıcı mantıksal gruptan ayrıldı: {group}");
            return;
        }

        try
        {
            var streamKey = GetBinanceStreamKey(group);
            await _streamManager.DecrementAndUnsubscribeIfNecessaryAsync(streamKey);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Gruptan ayrılma hatası: {group} -> {ex.Message}");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items.TryGetValue("UserGroups", out var userGroupsObject) &&
            userGroupsObject is ConcurrentBag<string> userGroups)
        {
            foreach (var group in userGroups)
            {
                if (group == VBorsa_API.Infrastructure.Helpers.Groups.MarketAllTrades)
                {
                    continue;
                }

                try
                {
                    var streamKey = GetBinanceStreamKey(group);
                    await _streamManager.DecrementAndUnsubscribeIfNecessaryAsync(streamKey);
                }
                catch (ArgumentException)
                {
                    
                }
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}