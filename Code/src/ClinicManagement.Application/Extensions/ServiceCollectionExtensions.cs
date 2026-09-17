using ClinicManagement.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagement.Application.Extensions;

/// <summary>Extension methods for registering application services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registers all application services with the DI container.</summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        return services;
    }
}
