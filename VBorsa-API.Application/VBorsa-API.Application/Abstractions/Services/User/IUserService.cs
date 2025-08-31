using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.User;
using VBorsa_API.Core.Entities.Identity;

namespace VBorsa_API.Application.Abstractions.Services.User;

public interface IUserService
{
    Task<Result> CreateUserAsync(CreateUserRequestDTO createUserRequestDTO);


    Task<DateTime> UpsertRefreshTokenAsync(AppUser user, string refreshToken,
        int refreshTokenExtraSeconds);

    Task<UpdateProfileDTO> GetProfileDetailsById(string userId);
    Task<UserDTO> GetUserDetailsById(string userId);
    Task<Result> UpdateProfileAsync(string id, UpdateProfileDTO updateProfileDto);
    Task<IReadOnlyList<UserDTO>> GetUsersAsync();
    Task<Result> UpdateUserAsync(string? id, UpdateUserDTO updateUserDTO);
    Task<Result> DeleteUserAsync(DeleteUserDTO deleteUserDTO);
    Task<Result> DeleteProfileAsync(string id);

    Task<Result> AssignRolesAsync(RoleDTO roleDTO);
}