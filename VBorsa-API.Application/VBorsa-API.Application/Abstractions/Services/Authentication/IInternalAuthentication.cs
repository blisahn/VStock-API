namespace VBorsa_API.Application.Abstractions.Services.Authentication;

public interface IInternalAuthentication
{
    Task<DTOs.Token> InternalLoginAsync(string usernameOrEmail, string password, int tokenLifeTimeSeconds);
}