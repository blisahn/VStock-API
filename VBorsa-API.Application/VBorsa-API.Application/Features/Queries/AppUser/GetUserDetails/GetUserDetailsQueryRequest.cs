using MediatR;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.User;

namespace VBorsa_API.Application.Features.Queries.AppUser.GetUserDetails;

public class GetUserDetailsQueryRequest : IRequest<Result<UserDTO>>
{
    public string Id { get; set; }
}