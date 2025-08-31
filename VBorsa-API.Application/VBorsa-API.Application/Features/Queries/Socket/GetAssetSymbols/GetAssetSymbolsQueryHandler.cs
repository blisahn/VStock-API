using MediatR;
using VBorsa_API.Application.Abstractions.Services.Symbol;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Symbol;

namespace VBorsa_API.Application.Features.Queries.Socket.GetAssetSymbols;

public class GetAssetSymbolsQueryHandler : IRequestHandler<GetAssetSymbolsQueryRequest, Result<Paged<SymbolDto>>>
{
    private readonly ISymbolService _symbolService;

    public GetAssetSymbolsQueryHandler(ISymbolService symbolService)
    {
        _symbolService = symbolService;
    }

    public async Task<Result<Paged<SymbolDto>>> Handle(GetAssetSymbolsQueryRequest request,
        CancellationToken cancellationToken)
    {
        var all = await _symbolService.GetByClassType(request.AssetClass);
        var page = request.Page < 1 ? 10 : request.Page;
        var size = request.PageSize <= 0 ? 10 : request.PageSize;

        var items = all.Skip((page - 1) * size)
            .Take(size)
            .ToList();

        var paged = new Paged<SymbolDto>(
            items,
            all.Count,
            request.Page,
            request.PageSize
        );

        return Result<Paged<SymbolDto>>.Success(paged, "Semboller Listelendi Listelendi");
    }
}