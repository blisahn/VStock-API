using VBorsa_API.Core.Entities.Common;
using VBorsa_API.Core.Entities.Identity;

namespace VBorsa_API.Core.Entities;

public class Holding : BaseEntity
{
    public Guid UserId { get; set; }
    public AppUser AppUser { get; set; }

    public Guid SymbolId { get; set; }
    public Symbol Symbol { get; set; }

    public decimal Quantity { get; set; }
    public decimal? AvgCost { get; set; }
    public decimal NetWorth { get; set; }
}