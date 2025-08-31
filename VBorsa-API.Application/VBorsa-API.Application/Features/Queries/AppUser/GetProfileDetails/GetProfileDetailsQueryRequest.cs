using MediatR;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.User;

namespace VBorsa_API.Application.Features.Queries.AppUser.GetProfileDetails;

public class GetProfileDetailsQueryRequest : IRequest<Result<UpdateProfileDTO>>
{
}