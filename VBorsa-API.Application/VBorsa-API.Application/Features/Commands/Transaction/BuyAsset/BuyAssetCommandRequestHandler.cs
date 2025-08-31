using MediatR;
using VBorsa_API.Application.Abstractions.Services.Asset;
using VBorsa_API.Application.DTOs.Asset;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.Transaction.BuyAsset;

public class BuyAssetCommandRequestHandler : IRequestHandler<BuyAssetCommandRequest, Result>
{
    private readonly IAssetService _assetService;

    public BuyAssetCommandRequestHandler(IAssetService assetService)
    {
        _assetService = assetService;
    }

    public async Task<Result> Handle(BuyAssetCommandRequest request, CancellationToken cancellationToken)
    {
        var dto = new BuyAssetRequestDto()
        {
            Amount = request.Amount,
            Symbol = request.Symbol,
            AssetClass = request.AssetClass
        };

        var res = await _assetService.BuyAsset(dto);
        return res.Succeeded
            ? Result.Success(res.Message)
            : Result.Failure(res.Message);
    }
}