using MediatR;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.Socket.CreateAssetSymbol;

public class CreateAssetSymbolCommandRequest : IRequest<Result>
{
    public string Source { get; set; } 
    public string Code { get; set; }
    public string AssetClass { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisibleForNonLogin { get; set; }
}