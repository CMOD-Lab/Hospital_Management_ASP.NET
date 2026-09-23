using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Infrastructure.Data;
using ClinicManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ClinicManagementSystem.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

        var connectionString = BuildPostgreSqlConnectionString(configuration);

        services.AddDbContext<ClinicManagementDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "public");
                    npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                })
                .UseSnakeCaseNamingConvention());

        services.AddIdentityCore<IdentityUser>()
            .AddEntityFrameworkStores<ClinicManagementDbContext>();

        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        return services;
    }

    private static string BuildPostgreSqlConnectionString(IConfiguration configuration)
    {
        var configuredConnectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(configuredConnectionString) && configuredConnectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
        {
            return configuredConnectionString;
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = configuration["Database:Host"] ?? "localhost",
            Port = int.TryParse(configuration["Database:Port"], out var port) ? port : 5432,
            Database = configuration["Database:Name"] ?? "clinic_management_system_db",
            Username = configuration["Database:Username"] ?? "postgres",
            Password = configuration["Database:Password"] ?? "postgres",
            Pooling = true,
            MinPoolSize = int.TryParse(configuration["Database:MinPoolSize"], out var minPoolSize) ? minPoolSize : 0,
            MaxPoolSize = int.TryParse(configuration["Database:MaxPoolSize"], out var maxPoolSize) ? maxPoolSize : 100,
            Timeout = int.TryParse(configuration["Database:Timeout"], out var timeout) ? timeout : 15,
            CommandTimeout = int.TryParse(configuration["Database:CommandTimeout"], out var commandTimeout) ? commandTimeout : 30,
            IncludeErrorDetail = true
        };

        return builder.ConnectionString;
    }
}
