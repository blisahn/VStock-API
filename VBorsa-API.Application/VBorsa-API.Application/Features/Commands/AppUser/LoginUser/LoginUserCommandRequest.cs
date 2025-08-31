using MediatR;
using VBorsa_API.Application.DTOs;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandRequest : IRequest<Result<Token>>
{
    public string UsernameOrEmail { get; init; }
    public string Password { get; init; }
}