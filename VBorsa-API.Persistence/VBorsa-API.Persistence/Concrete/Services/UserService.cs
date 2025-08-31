using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VBorsa_API.Application.Abstractions.Services.User;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.User;
using VBorsa_API.Application.Exceptions;
using VBorsa_API.Core.Entities.Identity;

namespace VBorsa_API.Persistence.Concrete.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<AppUser> userManager,
        IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<Result> AssignRolesAsync(RoleDTO roleDto)
    {
        var adminId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(roleDto.Id) ??
                   throw new UserNotFoundException("Rol atanacak kullanici bulunamadi");
        var current = await _userManager.GetRolesAsync(user);
        var removeResponse = await _userManager.RemoveFromRolesAsync(user, current);
        if (!removeResponse.Succeeded)
            throw new AssignRoleException("Gecmis roller kaldirilerken bir hata meydana geldi");
        var addRoleResult = await _userManager.AddToRolesAsync(user, roleDto.Roles);
        if (!addRoleResult.Succeeded)
            throw new AssignRoleException("Rolleri eklerken beklenmedik bir hata ile karsilasildi");

        _logger.LogWarning(
            "Admin bir kullanıcının rollerini güncelledi. AdminId: {AdminId}, TargetUserId: {TargetUserId}, OldRoles: {OldRoles}, NewRoles: {NewRoles}",
            adminId,
            user.Id,
            string.Join(",", current),
            string.Join(",", roleDto.Roles));
        return Result.Success("Roller basari ile guncellendi");
    }


    public async Task<UpdateProfileDTO> GetProfileDetailsById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new UserNotFoundException("Goruntulenecek kullanici bulunamadi");
        var response = new UpdateProfileDTO
        {
            FullName = user.FullName,
            Email = user.Email!,
            Username = user.UserName!
        };
        return response;
    }

    public async Task<UserDTO> GetUserDetailsById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new UserNotFoundException("Goruntulenecek kullanici bulunamadi");
        var response = new UserDTO
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            UserName = user.UserName!
        };
        return response;
    }

    public async Task<IReadOnlyList<UserDTO>> GetUsersAsync()
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .ToListAsync();
        if (users.Count == 0) throw new UserNotFoundException("Herhangi bir kayitli kullanici bulunamamaktadir");
        var results = new List<UserDTO>(users.Count);
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            results.Add(new UserDTO
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName!,
                UserName = user.UserName!,
                Roles = roles
            });
        }

        return results;
    }

    public async Task<Result> UpdateUserAsync(string? id, UpdateUserDTO updateUserDto)
    {
        var adminId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(id!) ??
                   throw new UserNotFoundException("Guncellenecek kullanici bulunamadi");
        user.UserName = updateUserDto.Username!;
        user.FullName = updateUserDto.FullName!;
        user.Email = updateUserDto.Email!;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new UpdateException("Kullanici guncellemesi yapilirken bir hata ile karsilasildi");
        _logger.LogInformation(
            $"Admin ID: {adminId}  bir kullanici guncelledi: UserID: {updateUserDto.Id}");
        return Result.Success("Kullanici  basari ile guncellenmistir.");
    }

    public async Task<Result> UpdateProfileAsync(string id, UpdateProfileDTO updateProfileDto)
    {
        var user = await _userManager.FindByIdAsync(id) ??
                   throw new UserNotFoundException(
                       "Profilinizi guncellerken, ilgili kullanici buluamadi, bir hata ile karsilasildi");
        user.UserName = updateProfileDto.Username!;
        user.Email = updateProfileDto.Email!;
        user.FullName = updateProfileDto.FullName!;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) throw new UpdateException("Profilinizi guncellerken bir hata ile kasilsildi.");
        _logger.LogInformation($"Kullanici guncellendi: UserID: {id}");
        return Result.Success("Profil basari ile guncellenmistir.");
    }


    public async Task<Result> DeleteUserAsync(DeleteUserDTO deleteUserDto)
    {
        var adminId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userToDelete = await _userManager.FindByIdAsync(deleteUserDto.UserId) ??
                           throw new UserNotFoundException("Silinecek kullanici bulunamadi");
        var response = await _userManager.DeleteAsync(userToDelete);


        if (!response.Succeeded)
        {
            _logger.LogError("Kullanıcı silinirken bir hata oluştu. AdminId: {AdminId}, TargetUserId: {TargetUserId}",
                adminId, deleteUserDto.UserId);
            throw new UserDeleteException("Kullaniciyi silerken bir hata ile karsilasildi");
        }

        _logger.LogWarning("Admin bir kullanıcıyı sildi. AdminId: {AdminId}, DeletedUserId: {DeletedUserId}",
            adminId,
            deleteUserDto.UserId);

        return Result.Success("Kullanici basari ile silinmistir.");
    }

    public async Task<Result> DeleteProfileAsync(string id)
    {
        var userToDelete = await _userManager.FindByIdAsync(id) ??
                           throw new UserNotFoundException("Silinecek profil bulunamadi");
        var response = await _userManager.DeleteAsync(userToDelete);
        if (!response.Succeeded) throw new UserDeleteException("Profili silerken bir hata ile karsilasildi");
        _logger.LogWarning($"Kullanici silindi: UserID: {id}");
        return Result.Success("Profil basari ile silinmistir.");
    }


    public async Task<DateTime> UpsertRefreshTokenAsync(
        AppUser user,
        string refreshToken,
        int refreshTokenLifetimeSeconds)
    {
        if (user is null) throw new UserNotFoundException("Kullanıcı bulunamadı.");
        var newExpiresAtUtc = DateTime.UtcNow.AddSeconds(refreshTokenLifetimeSeconds);
        user.RefreshToken = refreshToken;
        user.RefreshTokenEndDate = newExpiresAtUtc;
        var userUpdate = await _userManager.UpdateAsync(user);
        if (!userUpdate.Succeeded)
            throw new RegisterException(ExceptionHelper.GetCustomErrorMessage(userUpdate.Errors));

        return newExpiresAtUtc;
    }


    public async Task<Result> CreateUserAsync(CreateUserRequestDTO createUserRequestDto)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = createUserRequestDto.Username,
            FullName = createUserRequestDto.Name,
            Email = createUserRequestDto.Email,
        };
        var createResult = await _userManager.CreateAsync(user, createUserRequestDto.Password);
        await _userManager.AddToRolesAsync(user, new[] { "User" });
        if (!createResult.Succeeded)
            throw new UserCreationFailedException("Kullanici olusturulurken bir hata ile karsilasildi");

        return Result.Success("Kullanıcı oluşturuldu");
    }
}