using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VBorsa_API.Application.Features.Commands.Socket.CreateAssetSymbol;
using VBorsa_API.Application.Features.Commands.Socket.UpdateAssetVisibility;
using VBorsa_API.Application.Features.Queries.Socket.GetAssetDetails;
using VBorsa_API.Application.Features.Queries.Socket.GetAssetSymbols;
using VBorsa_API.Application.Features.Queries.Socket.GetCryptoData;
using VBorsa_API.Core.Entities.Identity.Helpers;
using VBorsa_API.Presentation.Extensions;
using VBorsa_API.Presentation.Monitoring;

namespace VBorsa_API.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SocketController : ControllerBase
{
    private readonly IMediator _mediator;

    public SocketController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet("[action]")]
    [Authorize(Policy = Permissions.EditProfile)]
    public async Task<IActionResult> GetCryptoKlineData(
        [FromQuery] GetCryptoKlineDataQueryRequest getCryptoKlineDataQueryRequest)
    {
        var response = await _mediator.Send(getCryptoKlineDataQueryRequest);
        return response.ToActionResult();
    }

    [Authorize(Policy = Permissions.SettingsManage)]
    [HttpPost("[action]")]
    public async Task<IActionResult> CreateAssetSymbol(CreateAssetSymbolCommandRequest createAssetSymbolCommandRequest)
    {
        var response = await _mediator.Send(createAssetSymbolCommandRequest);
        if (response.Succeeded)
        {
            AppMetrics.CreatedAssets.Inc();
        }
        return response.ToActionResult();
    }


    [Authorize(Policy = Permissions.SettingsView)]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAssetSymbols(
        [FromQuery] GetAssetSymbolsQueryRequest getAssetSymbolsQueryRequest)
    {
        var response = await _mediator.Send(getAssetSymbolsQueryRequest);
        return response.ToActionResult();
    }

    [HttpGet("[action]/{Id:guid}")]
    [Authorize(Policy = Permissions.SettingsManage)]
    public async Task<IActionResult> GetAssetDetails(
        [FromRoute] GetAssetDetailsQueryRequest getAssetDetailsQueryRequest)
    {
        var response = await _mediator.Send(getAssetDetailsQueryRequest);
        return response.ToActionResult();
    }

    [HttpPut("[action]")]
    [Authorize(Policy = Permissions.SettingsManage)]
    public async Task<IActionResult> UpdateAssetVisibility(
        [FromBody] UpdateAssetVisibilityCommandRequest updateAssetVisibilityCommandRequest)
    {
        var response = await _mediator.Send(updateAssetVisibilityCommandRequest);
        
        return response.ToActionResult();
    }
}