using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;
using VBorsa_API.Application;
using VBorsa_API.Infrastructure;
using VBorsa_API.Infrastructure.Helpers;
using VBorsa_API.Persistence;
using VBorsa_API.Persistence.Seed;
using VBorsa_API.Presentation.Handlers.CryptoNotification.Binance;
using VBorsa_API.Presentation.Handlers.CryptoNotification.FinHub;
using VBorsa_API.Presentation.Hubs;
using VBorsa_API.Presentation.Hubs.Crypto;
using VBorsa_API.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddApplicationServices();
builder.Services.AddSignalR();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(FinHubTradeDataUpdateNotificationHandler).Assembly));
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(BinanceTradeDataUpdateNotificationHandler).Assembly));
builder.Services.AddPersistenceServisces(builder.Configuration);
builder.Services.AddInfraStructureServices();
builder.Services.AddCryptoBackgroundService(SocketProviders.BINANCE);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VBorsa API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.  Enter 'Bearer' space and then your token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod()
        .AllowCredentials()
));

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseRouting();
app.UseHttpMetrics();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>(); 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseMiddleware<EnsureUserExistsMiddleware>();
app.UseAuthorization();
app.MapMetrics();
app.MapHub<PrivateCryptoHub>("/private-crypto-hub").RequireAuthorization();
app.MapHub<PublicCryptoHub>("/public-crypto-hub");

app.MapControllers();
await IdentitySeeder.SeedAsync(app.Services);
app.Run();