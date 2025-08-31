using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using VBorsa_API.Application.Abstractions.Services.Symbol;

namespace VBorsa_API.Application;

public static class ServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection collection)
    {
        collection.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()); });
    }
}