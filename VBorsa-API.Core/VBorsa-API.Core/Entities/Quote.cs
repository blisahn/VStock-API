using VBorsa_API.Core.Entities.Common;

namespace VBorsa_API.Core.Entities;

public class Quote : BaseEntity
{
    public Guid SymbolId { get; set; }
    public Symbol Symbol { get; set; } = null!;
    public decimal LastPrice { get; set; } // numeric(38, 12) karşılığı
    public DateTimeOffset RetrievedAt { get; set; }
}