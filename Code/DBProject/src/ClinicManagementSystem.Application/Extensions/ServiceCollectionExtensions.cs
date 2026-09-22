using ClinicManagementSystem.Application.Mappings;
using ClinicManagementSystem.Application.Services;
using ClinicManagementSystem.Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagementSystem.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddValidatorsFromAssemblyContaining<MappingProfile>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IStaffMemberService, StaffMemberService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        return services;
    }
}
