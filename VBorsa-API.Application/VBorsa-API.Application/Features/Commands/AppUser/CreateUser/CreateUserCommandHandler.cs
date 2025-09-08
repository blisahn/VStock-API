using MediatR;
using Microsoft.Extensions.Logging;
using VBorsa_API.Application.Abstractions.Services.User;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.User;

namespace VBorsa_API.Application.Features.Commands.AppUser.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, Result>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
    {
        var dto = new CreateUserRequestDTO
        {
            Name = request.FullName,
            Email = request.Email,
            Password = request.Password,
            Username = request.Username
        };
        var res = await _userService.CreateUserAsync(dto);
        return res.Succeeded
            ? Result.Success("Test")
            : Result.Failure(res.Message);
    }
}