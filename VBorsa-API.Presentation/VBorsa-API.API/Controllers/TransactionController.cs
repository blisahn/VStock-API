using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VBorsa_API.Application.Features.Commands.Transaction.BuyAsset;
using VBorsa_API.Application.Features.Commands.Transaction.CreateDeposit;
using VBorsa_API.Application.Features.Commands.Transaction.SellAsset;
using VBorsa_API.Application.Features.Queries.Transaction.GetTransactions;
using VBorsa_API.Presentation.Extensions;
using VBorsa_API.Presentation.Monitoring;

namespace VBorsa_API.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> GetTransactions()
    {
        var response = await _mediator.Send(new GetTransactionQueryRequest());
        return response.ToActionResult();
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> BuyAsset(
        [FromBody] BuyAssetCommandRequest buyAssetCommandRequest)
    {
        var response = await _mediator.Send(buyAssetCommandRequest);
        return response.ToActionResult();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SellAsset(
        [FromBody] SelAssetCommandRequest sellAssetCommandRequest)
    {
        var response = await _mediator.Send(sellAssetCommandRequest);
        return response.ToActionResult();
    }
    

}