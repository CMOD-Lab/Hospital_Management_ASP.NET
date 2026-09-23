using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ClinicManagementSystem.Infrastructure.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfrastructure_RegistersRepositoriesAndDbContext()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=user;Password=pass"
        }).Build();

        services.AddLogging();
        services.AddInfrastructure(configuration);

        Assert.Contains(services, x => x.ServiceType == typeof(IDoctorRepository));
        Assert.Contains(services, x => x.ServiceType == typeof(IPatientRepository));
        Assert.Contains(services, x => x.ServiceType == typeof(IAppointmentRepository));
        Assert.Contains(services, x => x.ServiceType == typeof(IDepartmentRepository));
    }

    [Fact]
    public void AddInfrastructure_UsesDatabaseSettingsWhenConnectionStringMissing()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Database:Host"] = "db",
            ["Database:Port"] = "5555",
            ["Database:Name"] = "clinic",
            ["Database:Username"] = "user",
            ["Database:Password"] = "pass"
        }).Build();

        services.AddInfrastructure(configuration);

        Assert.Contains(services, x => x.ServiceType == typeof(IDoctorRepository));
    }
}
