namespace VBorsa_API.Application.DTOs.Transaction;

public class CreateDepositDto
{
    public Guid UserId { get; set; }
    public  string TransactionType { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public Guid SymbolId { get; set; }
    public string Code { get; set; }
}