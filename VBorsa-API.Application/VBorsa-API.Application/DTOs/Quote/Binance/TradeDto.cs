namespace VBorsa_API.Application.DTOs.Quote.Binance;

public enum TradeSide
{
    Buy,
    Sell
}

public class TradeDto
{
    public required string Symbol { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public DateTime RetrievedAt { get; set; }
    public TradeSide Side { get; set; } // <-- EK
}

public class OrderBookDto
{
    public required string Symbol { get; set; }
    public required List<OrderBookLevelDto> Bids { get; set; }
    public required List<OrderBookLevelDto> Asks { get; set; }
}

public class OrderBookLevelDto
{
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
}

public class KlineDataDto
{
    public long? EventTime { get; set; }
    public required string Symbol { get; set; }
    public DateTime OpenTime { get; set; }
    public DateTime CloseTime { get; set; }
    public decimal? OpenPrice { get; set; }
    public decimal HighPrice { get; set; }
    public decimal LowPrice { get; set; }
    public decimal? ClosePrice { get; set; }
    public decimal Volume { get; set; }
    public decimal QuoteAssetVolume { get; set; }
    public int NumberOfTrades { get; set; }
    public bool? IsClosed { get; set; }
    public string Interval { get; set; }
}