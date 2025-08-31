namespace VBorsa_API.Application.DTOs.Holding;

public class UserHoldingDto
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public decimal MarketPrice { get; set; }
    public decimal ChangePercent { get; set; }
    public decimal Quantity { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal AverageCost { get; set; }
    public decimal ProfitLoss { get; set; }
}
