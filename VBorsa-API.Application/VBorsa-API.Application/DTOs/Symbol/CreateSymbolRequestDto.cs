namespace VBorsa_API.Application.DTOs.Symbol;

public class CreateSymbolRequestDto
{
    public string Source { get; set; }
    public string Code { get; set; }
    public string AssetClass { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisibleForNonLogin { get; set; }
}