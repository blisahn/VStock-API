using MediatR;
using VBorsa_API.Application.Abstractions.Services.Asset;
using VBorsa_API.Application.DTOs.Asset;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.Transaction.SellAsset;

public class SelAssetCommandRequestHandler : IRequestHandler<SelAssetCommandRequest, Result>
{
    private readonly IAssetService _assetService;

    public SelAssetCommandRequestHandler(IAssetService assetService)
    {
        _assetService = assetService;
    }

    public async Task<Result> Handle(SelAssetCommandRequest request, CancellationToken cancellationToken)
    {
        var dto = new SellAssetRequestDto()
        {
            Amount = request.Amount,
            Symbol = request.Symbol,
            AssetClass = request.AssetClass
        };

        var res = await _assetService.SelAsset(dto);
        return res.Succeeded
            ? Result.Success(res.Message)
            : Result.Failure(res.Message);
    }
}