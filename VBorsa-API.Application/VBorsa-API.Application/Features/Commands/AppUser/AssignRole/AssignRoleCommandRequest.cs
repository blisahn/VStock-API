using MediatR;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.AppUser.AssignRole;

public class AssignRoleCommandRequest : IRequest<Result>
{
    public string Id { get; set; }
    public IEnumerable<string> Roles { get; set; }
}