using MediatR;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.AppUser.DeleteUser;

public class DeleteUserCommandRequest : IRequest<Result>
{
    public string UserId { get; set; }
}