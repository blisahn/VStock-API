using MediatR;
using VBorsa_API.Application.Abstractions.Services.User;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.User;

namespace VBorsa_API.Application.Features.Queries.AppUser.GetAllUser;

public class GetAllUserQueryHandler : IRequestHandler<GetAllUsersQueryRequest, Result<Paged<UserDTO>>>
{
    private readonly IUserService _userService;

    public GetAllUserQueryHandler(IUserService userService)
    {
        _userService = userService;
    }


    public async Task<Result<Paged<UserDTO>>> Handle(GetAllUsersQueryRequest request,
        CancellationToken cancellationToken)
    {
        var all = await _userService.GetUsersAsync();
        var page = request.Page < 1 ? 1 : request.Page;
        var size = request.PageSize <= 0 ? 10 : request.PageSize;
        var items = all.Skip((page - 1) * size).Take(size)
            .ToList();

        var paged = new Paged<UserDTO>(
            items,
            all.Count,
            page,
            size
        );
        return Result<Paged<UserDTO>>.Success(paged, "Kullanıcılar Listelendi");
    }
}