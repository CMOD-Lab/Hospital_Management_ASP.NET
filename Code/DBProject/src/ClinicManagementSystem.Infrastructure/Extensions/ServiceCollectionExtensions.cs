using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Infrastructure.Persistence;
using ClinicManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagementSystem.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\mssqllocaldb;Database=ClinicManagementSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        services.AddDbContext<ClinicDbContext>(options => options.UseInMemoryDatabase("ClinicManagementSystemDb"));
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IStaffMemberRepository, StaffMemberRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        return services;
    }
}
