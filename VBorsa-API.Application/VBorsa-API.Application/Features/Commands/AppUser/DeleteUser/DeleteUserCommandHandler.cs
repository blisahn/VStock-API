using MediatR;
using VBorsa_API.Application.Abstractions.Services.User;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.User;

namespace VBorsa_API.Application.Features.Commands.AppUser.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommandRequest, Result>
{
    private readonly IUserService _userService;

    public DeleteUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> Handle(DeleteUserCommandRequest request, CancellationToken cancellationToken)
    {
        var dto = new DeleteUserDTO
        {
            UserId = request.UserId
        };
        var deleteResult = await _userService.DeleteUserAsync(dto);
        return deleteResult.Succeeded
            ? Result.Success(deleteResult.Message)
            : Result.Failure(deleteResult.Message);
    }
}