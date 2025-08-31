using MediatR;
using VBorsa_API.Application.Abstractions.Services.Symbol;
using VBorsa_API.Application.Abstractions.Services.User;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Socket.Data;

namespace VBorsa_API.Application.Features.Queries.Socket.GetAssetDetails;

public class GetAssetDetailsQueryHandler : IRequestHandler<GetAssetDetailsQueryRequest, Result<AssetSymbolDetailsDto>>
{
    private readonly ISymbolService _symbolService;

    public GetAssetDetailsQueryHandler(ISymbolService symbolService)
    {
        _symbolService = symbolService;
    }

    public async Task<Result<AssetSymbolDetailsDto>> Handle(GetAssetDetailsQueryRequest request,
        CancellationToken cancellationToken)
    {
        var asset = await _symbolService.GetAssetSymbolDetailsById(request.Id);
        return Result<AssetSymbolDetailsDto>.Success(asset, $"{asset.Code} ile alakali bilgiler getirildi");
    }
}