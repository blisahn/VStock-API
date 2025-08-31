using MediatR;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.AppUser.UpdateUser;

public class UpdateUserCommandRequest : IRequest<Result>
{
    public string Id { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
}