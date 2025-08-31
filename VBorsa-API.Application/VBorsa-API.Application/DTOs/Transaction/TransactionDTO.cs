namespace VBorsa_API.Application.DTOs.Transaction;

public class TransactionDto
{
    public string Symbol { get; set; }
    public string Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTimeOffset ExecutedAt { get; set; }
}