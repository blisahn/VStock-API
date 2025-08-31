namespace VBorsa_API.Infrastructure.Helpers;

public static class Groups
{
    public static string MarketAllTrades => "market_trades";
    public static string Trade(string symbol) =>
        $"trade_{symbol.ToUpper()}";

    public static string Depth(string symbol) =>
        $"depth_{symbol.ToUpper()}";

    public static string Kline(string symbol, string interval) =>
        $"kline_{symbol.ToUpper()}_{interval.ToLower()}";
}