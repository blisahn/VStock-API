using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using VBorsa_API.Application.Abstractions.Services.Transaction;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Transaction;

namespace VBorsa_API.Application.Features.Queries.Transaction.GetTransactions;

public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQueryRequest, Result<Paged<TransactionDto>>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITransactionService _transactionService;

    public GetTransactionQueryHandler(IHttpContextAccessor httpContextAccessor, ITransactionService transactionService)
    {
        _httpContextAccessor = httpContextAccessor;
        _transactionService = transactionService;
    }

    public async Task<Result<Paged<TransactionDto>>> Handle(GetTransactionQueryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var all = await _transactionService.GetTransactionsByIdAsync(userId);
        var page = request.Page < 1 ? 1 : request.Page;
        var size = request.Page <= 0 ? 10 : request.PageSize;
        var items = all.Skip((page - 1) * size).Take(size)
            .ToList();

        var paged = new Paged<TransactionDto>(
            items,
            all.Count,
            page,
            size
        );

        return Result<Paged<TransactionDto>>.Success(paged, "Islemler listelendi");
    }
}