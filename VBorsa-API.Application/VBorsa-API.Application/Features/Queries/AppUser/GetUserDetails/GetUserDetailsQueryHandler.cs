using MediatR;
using VBorsa_API.Application.Abstractions.Services.User;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.User;

namespace VBorsa_API.Application.Features.Queries.AppUser.GetUserDetails;

public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQueryRequest, Result<UserDTO>>
{
    private readonly IUserService _userService;

    public GetUserDetailsQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<UserDTO>> Handle(GetUserDetailsQueryRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserDetailsById(request.Id);
        return Result<UserDTO>.Success(user, "Kullanici bilgileri getirildi");
    }
}