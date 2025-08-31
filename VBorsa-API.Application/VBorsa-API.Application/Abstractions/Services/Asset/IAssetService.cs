using VBorsa_API.Application.DTOs.Asset;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Abstractions.Services.Asset;

public interface IAssetService
{
    Task<Result> BuyAsset(BuyAssetRequestDto dto);
    Task<Result> SelAsset(SellAssetRequestDto dto);
}   