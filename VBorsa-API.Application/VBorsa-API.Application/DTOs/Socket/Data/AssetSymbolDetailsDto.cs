namespace VBorsa_API.Application.DTOs.Socket.Data;

public class AssetSymbolDetailsDto
{
    public string? Source { get; set; } 
    public string? Code { get; set; }
    public string? AssetClass { get; set; }
    public bool IsActive { get; set; }
}