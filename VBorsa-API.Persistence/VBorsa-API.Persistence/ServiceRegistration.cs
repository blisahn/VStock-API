using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using VBorsa_API.Application.Abstractions.Services.Asset;
using VBorsa_API.Application.Abstractions.Services.Authentication;
using VBorsa_API.Application.Abstractions.Services.Holding;
using VBorsa_API.Application.Abstractions.Services.Symbol;
using VBorsa_API.Application.Abstractions.Services.Token;
using VBorsa_API.Application.Abstractions.Services.Transaction;
using VBorsa_API.Application.Abstractions.Services.User;
using VBorsa_API.Application.Repositories.Holding;
using VBorsa_API.Application.Repositories.Symbol;
using VBorsa_API.Application.Repositories.Transaction;
using VBorsa_API.Core.Entities.Identity;
using VBorsa_API.Core.Entities.Identity.Helpers;
using VBorsa_API.Persistence.Concrete.Services;
using VBorsa_API.Persistence.Context;
using VBorsa_API.Persistence.Repositories.Holding;
using VBorsa_API.Persistence.Repositories.Symbol;
using VBorsa_API.Persistence.Repositories.Transaction;
using TokenHandler = VBorsa_API.Persistence.Concrete.Services.TokenHandler;

namespace VBorsa_API.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceServisces(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<VBorsaDbContext>(options =>
            options.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));
        // services.AddHostedService<ISymbolService,SymbolService>();
        services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<VBorsaDbContext>()
            .AddDefaultTokenProviders();

        var key = Encoding.UTF8.GetBytes(cfg["Token:SecurityKey"]!);
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = cfg["Token:Issuer"],
                    ValidAudience = cfg["Token:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier
                };

                opt.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/private-crypto-hub"))
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = ctx =>
                    {
                        ctx.NoResult();
                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorization(options =>
        {
            foreach (var p in Permissions.All)
                options.AddPolicy(p, policy => policy.RequireClaim("permission", p));
        });

        services.AddScoped<ITransactionReadRepository, TransactionReadRepository>();
        services.AddScoped<ITransactionWriteRepository, TransactionWriteRepository>();
        services.AddScoped<ISymbolReadRepository, SymbolReadRepository>();
        services.AddScoped<ISymbolWriteRepository, SymbolWriteRepository>();
        services.AddScoped<IHoldingReadRepository, HoldingReadRepository>();
        services.AddScoped<IHoldingWriteRepository, HoldingWriteRepository>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IInternalAuthentication, AuthenticationService>();
        services.AddScoped<ITokenHandler, TokenHandler>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ISymbolService, SymbolService>();
        services.AddScoped<IHoldingService, HoldingService>();
        services.AddScoped<IAssetService, AssetService>();
    }
}