using ClinicManagementSystem.Application.Interfaces;
using Xunit;

namespace ClinicManagementSystem.Application.Interfaces;

public class ServiceInterfacesTests
{
    [Fact]
    public void ServiceInterfaces_AreInterfaces()
    {
        Assert.True(typeof(IDoctorService).IsInterface);
        Assert.True(typeof(IPatientService).IsInterface);
        Assert.True(typeof(IAppointmentService).IsInterface);
        Assert.True(typeof(IDepartmentService).IsInterface);
    }
}
