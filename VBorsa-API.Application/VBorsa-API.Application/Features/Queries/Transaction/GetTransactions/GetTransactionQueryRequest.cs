using MediatR;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Transaction;

namespace VBorsa_API.Application.Features.Queries.Transaction.GetTransactions;

public class GetTransactionQueryRequest : IRequest<Result<Paged<TransactionDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 100;
}