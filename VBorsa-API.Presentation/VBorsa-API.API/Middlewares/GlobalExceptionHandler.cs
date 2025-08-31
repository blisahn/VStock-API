using System.Net;
using VBorsa_API.Application.Exceptions;

namespace VBorsa_API.Presentation.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            var statusCode = (int)HttpStatusCode.InternalServerError;
            object? response = null;
            switch (ex)
            {
                case UserNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    response = new { Title = "Islem Basarisiz", ex.Message };
                    break;
                case SymbolAlreadyExistsException:
                    statusCode = (int)HttpStatusCode.PreconditionFailed;
                    response = new { Title = "Gecersiz Sembol Bilgisi", ex.Message };
                    break;
                case RegisterException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new { Title = "Islem Basarisiz", ex.Message };
                    break;
                case ValidationException:
                    statusCode = (int)HttpStatusCode.UnprocessableEntity;
                    response = new { Title = "Hata", ex.Message };
                    break;
                case LoginAuthenticationException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new { Title = "Islem Basarisiz", ex.Message };
                    break;
                case RefreshTokenNotValidException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    response = new { Title = "Yetkilendirme Hatasi", ex.Message };
                    break;
                case AuthenticationException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    response = new { Title = "Yetkilendirma Hatasi", ex.Message };
                    break;
                case UpdateException:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    response = new { Title = "İşlem Başarısız", ex.Message };
                    break;
                case UserCreationFailedException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new { Title = "İşlem Başarısız", ex.Message };
                    break;
                case AssignRoleException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new { Title = "İşlem Başarısız", ex.Message };
                    break;
                case MarketListFailedException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new { Title = "Listeleme Hatası", ex.Message };
                    break;
                case AssetCreationExceptıon:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new { Title = "İşlem Başarısız", ex.Message };
                    break;
                case TransactionException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new { Title = "Islem Gerceklestirilemedi", ex.Message };
                    break;
                case BinanceSocketException:
                    statusCode = (int)HttpStatusCode.BadGateway;
                    response = new { Title = "Akis saglanamadi", ex.Message };
                    break;
                default:

                    response = new { Title = "Hata", ex.Message };
                    break;
            }

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}