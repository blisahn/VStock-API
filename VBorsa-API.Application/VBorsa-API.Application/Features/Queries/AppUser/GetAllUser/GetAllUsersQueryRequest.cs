using MediatR;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Symbol;
using VBorsa_API.Application.DTOs.User;

namespace VBorsa_API.Application.Features.Queries.AppUser.GetAllUser;

public class GetAllUsersQueryRequest : IRequest<Result<Paged<UserDTO>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}