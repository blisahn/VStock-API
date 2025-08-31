namespace VBorsa_API.Application.DTOs.Socket;

using System.Text.Json.Serialization;

public class FinHubSocketMessage
{
    [JsonPropertyName("data")] public List<FinHubTradeData> Data { get; set; } = [];
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
}

public class FinHubTradeData
{
    [JsonPropertyName("p")] public decimal Price { get; set; }
    [JsonPropertyName("s")] public string Symbol { get; set; } = string.Empty;
    [JsonPropertyName("t")] public long Timestamp { get; set; }
    [JsonPropertyName("v")] public decimal Volume { get; set; }
}