using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VBorsa_API.Application.Abstractions.Services.Transaction;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Transaction;
using VBorsa_API.Application.Repositories.Symbol;
using VBorsa_API.Core.Entities;

namespace VBorsa_API.Application.Features.Commands.Transaction.CreateDeposit;

public class CreateDepositCommandHandler : IRequestHandler<CreateDepositCommandRequest, Result>
{
    private readonly ITransactionService _transactionService;

    public CreateDepositCommandHandler(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }


    public async Task<Result> Handle(CreateDepositCommandRequest request, CancellationToken cancellationToken)
    {
        var dto = new CreateDepositDto
        {
            TransactionType = request.TransactionType,
            Price = request.Price,
            Quantity = 1,
            Code = request.Code
        };

        var res = await _transactionService.CreateDepositAsync(dto);
        return res.Succeeded
            ? Result.Success(res.Message)
            : Result.Failure(res.Message);
    }
}