namespace VBorsa_API.Application.DTOs.Holding;

public class CreateHoldingDto
{
    public string SymbolId { get; set; }
    public decimal Quantity { get; set; }
    public decimal AvgCost { get; set; }
    public decimal NetWorth { get; set; }
}

