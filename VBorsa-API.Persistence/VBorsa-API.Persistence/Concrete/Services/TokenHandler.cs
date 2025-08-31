using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VBorsa_API.Application.Abstractions.Services.Token;
using VBorsa_API.Application.DTOs;
using VBorsa_API.Core.Entities.Identity;

namespace VBorsa_API.Persistence.Concrete.Services;

public class TokenHandler : ITokenHandler
{
    private readonly IConfiguration _configuration;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public TokenHandler(UserManager<AppUser> userManager, IConfiguration configuration,
        RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _roleManager = roleManager;
    }

    public async Task<Token> CreateAccessToken(int second, AppUser user)
    {
        Token token = new();


        var roles = await _userManager.GetRolesAsync(user);

        var userClaims = await _userManager.GetClaimsAsync(user);
        var permissionClaims = new List<Claim>();
        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            var claim = await _roleManager.GetClaimsAsync(role!);
            permissionClaims.AddRange(claim.Where(c => c.Type == "permission"));
        }

        var allPermissions = userClaims
            .Where(c => c.Type == "permission")
            .Concat(permissionClaims)
            .GroupBy(c => c.Value)
            .Select(g => new Claim("permission", g.Key));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new("AspNet.Identity.SecurityStamp", user.SecurityStamp ?? string.Empty)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        claims.AddRange(allPermissions);

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]!));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddSeconds(second);
        token.Expiration = expiration;
        JwtSecurityToken securityToken = new(
            audience: _configuration["Token:Audience"],
            issuer: _configuration["Token:Issuer"],
            expires: expiration,
            notBefore: DateTime.UtcNow,
            signingCredentials: credentials,
            claims: claims
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return new Token
        {
            AccessToken = jwt,
            Expiration = expiration,
            RefreshToken = CreateRefreshToken(), 
            RefreshTokenExpiresAtUtc = expiration.AddSeconds(45)
        };
    }

    public string CreateRefreshToken()
    {
        var number = new byte[32];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(number);
        return Convert.ToBase64String(number);
    }
}