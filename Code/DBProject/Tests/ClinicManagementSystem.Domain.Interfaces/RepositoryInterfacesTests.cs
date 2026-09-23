using ClinicManagementSystem.Domain.Interfaces.Repositories;
using Xunit;

namespace ClinicManagementSystem.Domain.Interfaces.Repositories;

public class RepositoryInterfacesTests
{
    [Fact]
    public void RepositoryInterfaces_AreInterfaces()
    {
        Assert.True(typeof(IDoctorRepository).IsInterface);
        Assert.True(typeof(IPatientRepository).IsInterface);
        Assert.True(typeof(IAppointmentRepository).IsInterface);
        Assert.True(typeof(IDepartmentRepository).IsInterface);
    }
}
