using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VBorsa_API.Application.Abstractions.Services.Authentication;
using VBorsa_API.Application.Abstractions.Services.Token;
using VBorsa_API.Application.Abstractions.Services.User;
using VBorsa_API.Application.DTOs;
using VBorsa_API.Application.Exceptions;
using VBorsa_API.Core.Entities.Identity;

namespace VBorsa_API.Persistence.Concrete.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenHandler _tokenHandler;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserService _userService;


    public AuthenticationService(UserManager<AppUser> userManager, ITokenHandler tokenHandler,
        SignInManager<AppUser> signInManager,
        IUserService userService)
    {
        _userManager = userManager;
        _tokenHandler = tokenHandler;
        _signInManager = signInManager;
        _userService = userService;
    }

    public async Task<Token> InternalLoginAsync(string usernameOrEmail, string password, int accessTokenLifeTimeSeconds)
    {
        var user = await _userManager.FindByNameAsync(usernameOrEmail)
                   ?? await _userManager.FindByEmailAsync(usernameOrEmail)
                   ?? throw new UserNotFoundException("Lutfen once hesap acin.");


        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        if (!result.Succeeded)
            throw new AuthenticationException("Gecersiz kullanici adi, mail veya sifre, Oturum acilamadi.");

        var token = await _tokenHandler.CreateAccessToken(accessTokenLifeTimeSeconds, user);
        var refreshSeconds = 86400;
        var expiresAtUtc = await _userService.UpsertRefreshTokenAsync(
            user,
            token.RefreshToken!, 
            refreshSeconds
        );

        return new Token
        {
            AccessToken = token.AccessToken,
            Expiration = token.Expiration,
            RefreshToken = user.RefreshToken!,
            RefreshTokenExpiresAtUtc = expiresAtUtc 
        };
    }

    public async Task<Token> RefreshTokenLoginAsync(string refreshToken)
    {

        AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user == null) throw new UserNotFoundException("Yetkilendirilecek kullanici bulunamadi");
        
        var accessSeconds = 900;
        var refreshSeconds = 86400;
        var newToken = await _tokenHandler.CreateAccessToken(accessSeconds, user);
        var newExpiresAtUtc = await _userService.UpsertRefreshTokenAsync(
            user,
            newToken.RefreshToken!,
            refreshSeconds 
        );
        newToken.RefreshTokenExpiresAtUtc = newExpiresAtUtc;
        return newToken;
    }
}