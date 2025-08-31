using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using VBorsa_API.Application.Abstractions.Services.User;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.User;

namespace VBorsa_API.Application.Features.Commands.AppUser.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommandRequest, Result>
{
    private readonly IHttpContextAccessor _http;
    private readonly IUserService _userService;

    public UpdateProfileCommandHandler(IUserService userService, IHttpContextAccessor http)
    {
        _userService = userService;
        _http = http;
    }


    public async Task<Result> Handle(UpdateProfileCommandRequest request, CancellationToken cancellationToken)
    {
        var userId = _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var dto = new UpdateProfileDTO
        {
            Username = request.Username!,
            FullName = request.FullName!,
            Email = request.Email!
        };

        var updateProfileResponse = await _userService.UpdateProfileAsync(userId!, dto);
        return updateProfileResponse.Succeeded
            ? Result.Success(updateProfileResponse.Message)
            : Result.Failure(updateProfileResponse.Message);
    }
}