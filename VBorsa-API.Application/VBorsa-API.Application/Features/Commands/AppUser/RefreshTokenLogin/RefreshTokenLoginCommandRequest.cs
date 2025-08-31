using MediatR;
using VBorsa_API.Application.DTOs;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.AppUser.RefreshTokenLogin;

public class RefreshTokenLoginCommandRequest : IRequest<Result<Token>>
{
    public string RefreshToken { get; set; }
}