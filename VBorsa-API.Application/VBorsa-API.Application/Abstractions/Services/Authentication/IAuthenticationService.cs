namespace VBorsa_API.Application.Abstractions.Services.Authentication;

public interface IAuthenticationService : IInternalAuthentication
{
    Task<DTOs.Token> RefreshTokenLoginAsync(string refreshToken);
}