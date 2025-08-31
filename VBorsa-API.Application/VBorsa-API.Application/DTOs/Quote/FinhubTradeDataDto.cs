namespace VBorsa_API.Application.DTOs.Quote;

public class FinhubTradeDataDto
{
    public decimal Price { get; set; }
    public long RetrievedAt { get; set; }
    public string Symbol { get; set; }
    public string Source { get; set; }
    public string Provider { get; set; }
}