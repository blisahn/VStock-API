using MediatR;
using VBorsa_API.Application.Abstractions.Services.Holding;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Holding;

namespace VBorsa_API.Application.Features.Queries.Holding;

public class GetUserHoldingsQueryHandler : IRequestHandler<GetHoldingQueryRequest, Result<Paged<UserHoldingDto>>>
{
    private readonly IHoldingService _holdingService;

    public GetUserHoldingsQueryHandler(IHoldingService holdingService)
    {
        _holdingService = holdingService;
    }

    public  async Task<Result<Paged<UserHoldingDto>>> Handle(GetHoldingQueryRequest request, CancellationToken cancellationToken)
    {
        var all = await _holdingService.GetUserHoldingsAsync();
        var page = request.Page < 1 ? 1 : request.Page;
        var size = request.PageSize <= 0 ? 10 : request.PageSize;
        var items = all.Skip((page - 1) * size).Take(size)
            .ToList();

        var paged = new Paged<UserHoldingDto>(
            items,
            all.Count,
            page,
            size
        );
        return Result<Paged<UserHoldingDto>>.Success(paged, "Kullanıcılar Listelendi");
    }
}