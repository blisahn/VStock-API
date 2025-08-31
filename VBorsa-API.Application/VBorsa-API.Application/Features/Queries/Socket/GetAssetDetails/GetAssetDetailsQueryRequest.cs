using MediatR;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Socket.Data;

namespace VBorsa_API.Application.Features.Queries.Socket.GetAssetDetails;

public class GetAssetDetailsQueryRequest : IRequest<Result<AssetSymbolDetailsDto>>
{
    public string Id { get; set; }
}