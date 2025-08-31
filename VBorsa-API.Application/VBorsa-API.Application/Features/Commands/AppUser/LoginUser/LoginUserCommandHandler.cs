using MediatR;
using VBorsa_API.Application.Abstractions.Services.Authentication;
using VBorsa_API.Application.DTOs;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, Result<Token>>
{
    private readonly IAuthenticationService _authenticationService;

    public LoginUserCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<Token>> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
    {
        var token = await _authenticationService.InternalLoginAsync(
            request.UsernameOrEmail,
            request.Password,
            900
        );
        return Result<Token>.Success(token, "Giriş başarılı");
    }
}