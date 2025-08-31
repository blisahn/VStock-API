using MediatR;
using VBorsa_API.Application.Abstractions.Services.User;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.User;

namespace VBorsa_API.Application.Features.Commands.AppUser.AssignRole;

public class AssignRoleRequestHandler : IRequestHandler<AssignRoleCommandRequest, Result>
{
    private readonly IUserService _userService;

    public AssignRoleRequestHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> Handle(AssignRoleCommandRequest request, CancellationToken cancellationToken)
    {
        var dto = new RoleDTO
        {
            Id = request.Id,
            Roles = request.Roles
        };
        var res = await _userService.AssignRolesAsync(dto);
        return res.Succeeded
            ? Result.Success(res.Message)
            : Result.Failure(res.Message);
    }
}