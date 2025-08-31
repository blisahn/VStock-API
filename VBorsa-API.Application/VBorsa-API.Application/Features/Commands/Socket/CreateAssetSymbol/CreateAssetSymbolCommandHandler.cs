using MediatR;
using VBorsa_API.Application.Abstractions.Services.Symbol;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Symbol;
using VBorsa_API.Application.Repositories.Symbol;
using VBorsa_API.Core.Entities;

namespace VBorsa_API.Application.Features.Commands.Socket.CreateAssetSymbol;

public class CreateAssetSymbolCommandHandler : IRequestHandler<CreateAssetSymbolCommandRequest, Result>
{
    private readonly ISymbolService _symbolService;

    public CreateAssetSymbolCommandHandler(ISymbolService symbolService)
    {
        _symbolService = symbolService;
    }

    public async Task<Result> Handle(CreateAssetSymbolCommandRequest request, CancellationToken cancellationToken)
    {
        var symbol = new CreateSymbolRequestDto()
        {
            Code = request.Code,
            Source = request.Source,
            AssetClass = request.AssetClass,
            IsActive = request.IsActive,
            IsVisibleForNonLogin = request.IsVisibleForNonLogin,
        };
        return await _symbolService.CreateAssetSymbolAsync(symbol);
    }
    
}