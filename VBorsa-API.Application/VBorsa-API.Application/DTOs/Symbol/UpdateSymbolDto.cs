namespace VBorsa_API.Application.DTOs.Symbol;

public class UpdateSymbolDto
{
    public string Id { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisibleForNonLogin { get; set; }
}