using MediatR;
using VBorsa_API.Application.Abstractions.Services.Symbol;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Symbol;
using VBorsa_API.Application.Features.Notifications.Symbol;

namespace VBorsa_API.Application.Features.Commands.Socket.UpdateAssetVisibility;

public class UpdateAssetVisibilityCommandHandler : IRequestHandler<UpdateAssetVisibilityCommandRequest, Result>
{
    private readonly ISymbolService _symbolService;
    private readonly IMediator _mediator;
    public UpdateAssetVisibilityCommandHandler(ISymbolService symbolService, IMediator mediator)
    {
        _symbolService = symbolService;
        _mediator = mediator;
    }
    public async Task<Result> Handle(UpdateAssetVisibilityCommandRequest request, CancellationToken cancellationToken)
    {
        var id = request.Id;
        var dto = new UpdateSymbolDto()
        {
            Id = id,
            IsActive = request.IsActive,
            IsVisibleForNonLogin = request.IsVisibleForNonLogin
        };

        var updateUserResponse = await _symbolService.UpdateSymbolAsync(id, dto);
        if (updateUserResponse.Succeeded)
        {
            await _mediator.Publish(new PublicSymbolsChangedNotification(), cancellationToken);
        }
        return updateUserResponse.Succeeded
            ? Result.Success(updateUserResponse.Message)
            : Result.Failure(updateUserResponse.Message);
    }
}