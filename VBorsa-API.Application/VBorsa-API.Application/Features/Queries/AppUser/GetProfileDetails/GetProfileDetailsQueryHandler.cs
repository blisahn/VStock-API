using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using VBorsa_API.Application.Abstractions.Services.User;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.User;

namespace VBorsa_API.Application.Features.Queries.AppUser.GetProfileDetails;

public class
    GetProfileDetailsQueryHandler : IRequestHandler<GetProfileDetailsQueryRequest, Result<UpdateProfileDTO>>
{
    private readonly IHttpContextAccessor _http;
    private readonly IUserService _userService;

    public GetProfileDetailsQueryHandler(IUserService userService, IHttpContextAccessor http)
    {
        _userService = userService;
        _http = http;
    }

    public async Task<Result<UpdateProfileDTO>> Handle(GetProfileDetailsQueryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var profile = await _userService.GetProfileDetailsById(userId!);
        return Result<UpdateProfileDTO>.Success(profile, "Profil bilgileri getirildi");
    }
}