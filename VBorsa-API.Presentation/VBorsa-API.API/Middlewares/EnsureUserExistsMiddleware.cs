using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VBorsa_API.Core.Entities.Identity;

namespace VBorsa_API.Presentation.Middlewares;

public class EnsureUserExistsMiddleware
{
    private readonly RequestDelegate _next;

    public EnsureUserExistsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<AppUser> userManager)
    {
        var endpoint = context.GetEndpoint();
        var allowedAnonymous = endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null;
        var requiresAuthorize = endpoint?.Metadata.GetMetadata<IAuthorizeData>() != null;

        if (allowedAnonymous || !requiresAuthorize)
        {
            await _next(context);
            return;
        }

        if (context.User?.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        var userId = context.User!.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Kullanici kimligi eksik");
            return;
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Gecerli bir kullanici bulunmamaktadir.");
            return;
        }

        var tokenStamp = context.User.FindFirstValue("AspNet.Identity.SecurityStamp");
        if (!string.IsNullOrEmpty(tokenStamp) && user.SecurityStamp != tokenStamp)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Security stamp mismatch.");
            return;
        }
        await _next(context);
    }
}