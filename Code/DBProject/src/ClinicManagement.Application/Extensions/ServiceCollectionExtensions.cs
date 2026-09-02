using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagement.Application.Extensions;

/// <summary>
/// Extension methods for registering Application layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Application layer services with the DI container.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IStaffService, StaffService>();

        return services;
    }
}
