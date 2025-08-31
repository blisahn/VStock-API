using System.Text.Json.Serialization;

namespace VBorsa_API.Application.DTOs.Socket;

public class BinanceSocketMessage<T>
{
    [JsonPropertyName("stream")] public string Stream { get; set; }

    [JsonPropertyName("data")] public T Data { get; set; }
}

public class AggregateTradeData
{
    [JsonPropertyName("s")] public string Symbol { get; set; }
    [JsonPropertyName("p")] public string Price { get; set; }
    [JsonPropertyName("q")] public string Quantity { get; set; }
    [JsonPropertyName("T")] public long Timestamp { get; set; }
    [JsonPropertyName("m")] public bool IsBuyerMarketMaker { get; set; }
}

public class KlineData
{
    [JsonPropertyName("s")] public string Symbol { get; set; }
    [JsonPropertyName("E")] public long EventTime { get; set; }
    [JsonPropertyName("k")] public KlineInfo KlineInfo { get; set; }
}

public class KlineInfo
{
    [JsonPropertyName("t")] public long StartTime { get; set; }
    [JsonPropertyName("T")] public long CloseTime { get; set; }
    [JsonPropertyName("o")] public string OpenPrice { get; set; }
    [JsonPropertyName("c")] public string ClosePrice { get; set; }
    [JsonPropertyName("h")] public string HighPrice { get; set; }
    [JsonPropertyName("l")] public string LowPrice { get; set; }
    [JsonPropertyName("v")] public string Volume { get; set; }
    [JsonPropertyName("q")] public string QuoteAssetVolume { get; set; }
    [JsonPropertyName("n")] public int NumberOfTrades { get; set; }
    [JsonPropertyName("x")] public bool IsClosed { get; set; }
}

public class DepthData
{
    [JsonPropertyName("bids")] public List<List<string>> Bids { get; set; } = [];
    [JsonPropertyName("asks")] public List<List<string>> Asks { get; set; } = [];

    [JsonPropertyName("b")]
    public List<List<string>> B
    {
        get => Bids;
        set => Bids = value;
    }

    [JsonPropertyName("a")]
    public List<List<string>> A
    {
        get => Asks;
        set => Asks = value;
    }
}