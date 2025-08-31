using MediatR;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.Transaction.CreateDeposit;

public class CreateDepositCommandRequest : IRequest<Result>
{
    public string Code { get; set; }
    public string TransactionType { get; set; }
    public decimal Price { get; set; }
}