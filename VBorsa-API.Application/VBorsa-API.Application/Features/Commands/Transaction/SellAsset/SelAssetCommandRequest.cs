using MediatR;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.Transaction.SellAsset;

public class SelAssetCommandRequest:IRequest<Result>
{
    public decimal Amount { get; set; }
    public string Symbol { get; set; }
    public string AssetClass { get; set; }
}