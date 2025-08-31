using VBorsa_API.Core.Entities.Identity;

namespace VBorsa_API.Application.Abstractions.Services.Token;

public interface ITokenHandler
{
    Task<DTOs.Token> CreateAccessToken(int second, AppUser user);
}