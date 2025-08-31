using MediatR;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.AppUser.UpdateProfile;

public class UpdateProfileCommandRequest : IRequest<Result>
{
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
}