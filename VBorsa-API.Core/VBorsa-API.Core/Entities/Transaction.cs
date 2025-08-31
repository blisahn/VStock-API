using VBorsa_API.Core.Entities.Common;
using VBorsa_API.Core.Entities.Identity;

namespace VBorsa_API.Core.Entities;

public class Transaction : BaseEntity
{
    public Guid UserId { get; set; }
    public AppUser AppUser { get; set; } = null!;
    public Guid SymbolId { get; set; }
    public Symbol Symbol { get; set; } = null!;
    public string Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTimeOffset ExecutedAt { get; set; }
}