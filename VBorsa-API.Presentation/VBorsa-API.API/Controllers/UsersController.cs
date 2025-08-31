using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VBorsa_API.Application.Features.Commands.AppUser.AssignRole;
using VBorsa_API.Application.Features.Commands.AppUser.CreateUser;
using VBorsa_API.Application.Features.Commands.AppUser.DeleteProfile;
using VBorsa_API.Application.Features.Commands.AppUser.DeleteUser;
using VBorsa_API.Application.Features.Commands.AppUser.UpdateProfile;
using VBorsa_API.Application.Features.Commands.AppUser.UpdateUser;
using VBorsa_API.Application.Features.Queries.AppUser.GetAllUser;
using VBorsa_API.Application.Features.Queries.AppUser.GetProfileDetails;
using VBorsa_API.Application.Features.Queries.AppUser.GetUserDetails;
using VBorsa_API.Core.Entities.Identity.Helpers;
using VBorsa_API.Presentation.Extensions;
using VBorsa_API.Presentation.Monitoring;

namespace VBorsa_API.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommandRequest createUserCommandRequest)
    {
        var result = await _mediator.Send(createUserCommandRequest);
        if (result.Succeeded)
        {
            AppMetrics.RegisteredUsersCounter.Inc();
        }
        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetProfileDetails()
    {
        var response = await _mediator.Send(new GetProfileDetailsQueryRequest());
        return response.ToActionResult();
    }

    [HttpDelete("[action]")]
    [Authorize(Policy = Permissions.EditProfile)]
    public async Task<IActionResult> DeleteProfile()
    {
        var response = await _mediator.Send(new DeleteProfileCommandRequest());
        return response.ToActionResult();
    }

    [HttpPut("[action]")]
    [Authorize(Policy = Permissions.EditProfile)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommandRequest updateProfileCommandRequest)
    {
        var response = await _mediator.Send(updateProfileCommandRequest);
        return response.ToActionResult();
    }

    [HttpGet("[action]")]
    [Authorize(Policy = Permissions.UsersView)]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _mediator.Send(new GetAllUsersQueryRequest());
        return response.ToActionResult();
    }


    [HttpPut("[action]")]
    [Authorize(Policy = Permissions.UsersManage)]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommandRequest updateUserCommandRequest)
    {
        var response = await _mediator.Send(updateUserCommandRequest);
        return response.ToActionResult();
    }

    [HttpPut("[action]")]
    [Authorize(Policy = Permissions.UsersManage)]
    public async Task<IActionResult> AssignRole(AssignRoleCommandRequest assignRoleCommandRequest)
    {
        var response = await _mediator.Send(assignRoleCommandRequest);
        return response.ToActionResult();
    }

    [HttpDelete("[action]/{UserId:guid}")]
    [Authorize(Policy = Permissions.UsersManage)]
    public async Task<IActionResult> DeleteUser([FromRoute] DeleteUserCommandRequest deleteUserCommandRequest)
    {
        var response = await _mediator.Send(deleteUserCommandRequest);
        return response.ToActionResult();
    }

    [HttpGet("[action]/{Id:guid}")]
    [Authorize(Policy = Permissions.UsersManage)]
    public async Task<IActionResult> GetUserDetails([FromRoute] GetUserDetailsQueryRequest getUserDetailsQueryRequest)
    {
        var response = await _mediator.Send(getUserDetailsQueryRequest);
        return response.ToActionResult();
    }
}