using ClinicManagementSystem.Application.Extensions;
using ClinicManagementSystem.Application.Interfaces;
using ClinicManagementSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ClinicManagementSystem.Application.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplication_RegistersExpectedServices()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddApplication();

        Assert.Contains(services, x => x.ServiceType == typeof(IDoctorService) && x.ImplementationType == typeof(DoctorService));
        Assert.Contains(services, x => x.ServiceType == typeof(IPatientService) && x.ImplementationType == typeof(PatientService));
        Assert.Contains(services, x => x.ServiceType == typeof(IAppointmentService) && x.ImplementationType == typeof(AppointmentService));
        Assert.Contains(services, x => x.ServiceType == typeof(IDepartmentService) && x.ImplementationType == typeof(DepartmentService));
        Assert.Contains(services, x => x.ServiceType.Name.Contains("IMapper"));
    }
}
