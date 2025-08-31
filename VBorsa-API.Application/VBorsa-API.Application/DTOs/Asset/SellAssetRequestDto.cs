namespace VBorsa_API.Application.DTOs.Asset;

public class SellAssetRequestDto
{
    public decimal Amount { get; set; }
    public string Symbol { get; set; }
    public string AssetClass { get; set; }
}