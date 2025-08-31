using MediatR;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Holding;

namespace VBorsa_API.Application.Features.Queries.Holding;

public class GetHoldingQueryRequest: IRequest<Result<Paged<UserHoldingDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}