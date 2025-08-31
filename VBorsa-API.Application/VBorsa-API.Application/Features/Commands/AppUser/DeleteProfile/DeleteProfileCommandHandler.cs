using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using VBorsa_API.Application.Abstractions.Services.User;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.AppUser.DeleteProfile;

public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommandRequest, Result>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;

    public DeleteProfileCommandHandler(IUserService userService, IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(DeleteProfileCommandRequest request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var deleteResult = await _userService.DeleteProfileAsync(userId!);
        return deleteResult.Succeeded
            ? Result.Success(deleteResult.Message)
            : Result.Failure(deleteResult.Message);
    }
}