using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagement.Application.Extensions;

/// <summary>Application layer service registration extensions</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registers application layer services</summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper manually (without extension package)
        services.AddSingleton<IMapper>(sp =>
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            return config.CreateMapper();
        });

        // Register services
        services.AddScoped<IAuthService, Services.AuthService>();
        services.AddScoped<IPatientService, Services.PatientService>();
        services.AddScoped<IDoctorService, Services.DoctorService>();
        services.AddScoped<IAdminService, Services.AdminService>();
        services.AddScoped<IAppointmentService, Services.AppointmentService>();
        services.AddScoped<IDepartmentService, Services.DepartmentService>();

        return services;
    }
}
