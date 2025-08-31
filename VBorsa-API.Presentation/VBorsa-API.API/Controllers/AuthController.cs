using MediatR;
using Microsoft.AspNetCore.Mvc;
using VBorsa_API.Application.Features.Commands.AppUser.LoginUser;
using VBorsa_API.Application.Features.Commands.AppUser.RefreshTokenLogin;
using VBorsa_API.Presentation.Extensions;
using VBorsa_API.Presentation.Monitoring;

namespace VBorsa_API.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommandRequest loginUserCommandRequest)
    {
        var response = await _mediator.Send(loginUserCommandRequest);
        return response.ToActionResult();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RefreshTokenLogin(
        [FromBody] RefreshTokenLoginCommandRequest refreshTokenLoginCommandRequest)
    {
        var response = await _mediator.Send(refreshTokenLoginCommandRequest);
        return response.ToActionResult();
    }
}