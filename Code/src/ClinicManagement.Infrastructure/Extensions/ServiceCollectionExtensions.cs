using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagement.Infrastructure.Extensions;

/// <summary>Extension methods for registering infrastructure services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registers all infrastructure services with the DI container.</summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IAppointmentService, AppointmentService>();

        return services;
    }
}
