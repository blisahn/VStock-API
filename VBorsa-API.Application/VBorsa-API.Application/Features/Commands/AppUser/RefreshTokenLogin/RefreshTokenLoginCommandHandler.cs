using MediatR;
using VBorsa_API.Application.Abstractions.Services.Authentication;
using VBorsa_API.Application.DTOs;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.AppUser.RefreshTokenLogin;

public class
    RefreshTokenLoginCommandHandler : IRequestHandler<RefreshTokenLoginCommandRequest, Result<Token>>
{
    private readonly IAuthenticationService _authenticationService;

    public RefreshTokenLoginCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }


    public async Task<Result<Token>> Handle(RefreshTokenLoginCommandRequest request,
        CancellationToken cancellationToken)
    {
        var token = await _authenticationService.RefreshTokenLoginAsync(request.RefreshToken);
        return Result<Token>.Success(token, "Giriş başarılı");
    }
}