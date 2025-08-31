using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VBorsa_API.Application.Features.Commands.Transaction.BuyAsset;
using VBorsa_API.Application.Features.Commands.Transaction.CreateDeposit;
using VBorsa_API.Application.Features.Queries.Holding;
using VBorsa_API.Application.Features.Queries.Transaction.GetTransactions;
using VBorsa_API.Presentation.Extensions;

namespace VBorsa_API.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class HoldingController : ControllerBase
{
    private readonly IMediator _mediator;

    public HoldingController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> CreateDeposit(
        [FromBody] CreateDepositCommandRequest createDepositCommandRequest)
    {
        var response = await _mediator.Send(createDepositCommandRequest);
        return response.ToActionResult();
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetUserAssets()
    {
        var response = await _mediator.Send(new GetHoldingQueryRequest());
        return response.ToActionResult();
    }

}