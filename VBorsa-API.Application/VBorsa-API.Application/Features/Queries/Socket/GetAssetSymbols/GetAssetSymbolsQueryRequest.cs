using MediatR;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Symbol;

namespace VBorsa_API.Application.Features.Queries.Socket.GetAssetSymbols;

public class GetAssetSymbolsQueryRequest : IRequest<Result<Paged<SymbolDto>>>
{
    public string AssetClass { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}