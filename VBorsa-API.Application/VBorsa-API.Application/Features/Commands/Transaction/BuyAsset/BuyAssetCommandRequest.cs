using MediatR;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.Transaction.BuyAsset;

public class BuyAssetCommandRequest : IRequest<Result>
{
    public decimal Amount { get; set; }
    public string Symbol { get; set; }
    public string AssetClass { get; set; }
}
