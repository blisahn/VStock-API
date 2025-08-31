using VBorsa_API.Core.Entities.Common;

namespace VBorsa_API.Core.Entities;

public class Symbol : BaseEntity
{
    public string Source { get; set; } // "BINANCE", "NASDAQ"...
    public string Code { get; set; }
    public string AssetClass { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisibleForNonLogin { get; set; }
}