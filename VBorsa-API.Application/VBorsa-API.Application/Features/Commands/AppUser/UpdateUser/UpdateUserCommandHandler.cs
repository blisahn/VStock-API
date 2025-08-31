using MediatR;
using VBorsa_API.Application.Abstractions.Services.User;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.User;

namespace VBorsa_API.Application.Features.Commands.AppUser.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommandRequest, Result>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> Handle(UpdateUserCommandRequest request, CancellationToken cancellationToken)
    {
        var dto = new UpdateUserDTO
        {
            Id = request.Id,
            Username = request.Username,
            FullName = request.FullName,
            Email = request.Email
        };
        var updateUserResponse = await _userService.UpdateUserAsync(dto.Id, dto);
        return updateUserResponse.Succeeded
            ? Result.Success(updateUserResponse.Message)
            : Result.Failure(updateUserResponse.Message);
    }
}