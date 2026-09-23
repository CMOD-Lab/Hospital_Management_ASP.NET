using ClinicManagementSystem.Domain.Entities;
using Xunit;

namespace ClinicManagementSystem.Domain.Entities;

public class DepartmentTests
{
    [Fact]
    public void Constructor_InitializesDefaults()
    {
        var department = new Department();

        Assert.Equal(string.Empty, department.Name);
        Assert.Null(department.Description);
        Assert.NotNull(department.Doctors);
        Assert.Empty(department.Doctors);
    }

    [Fact]
    public void Properties_CanBeAssigned()
    {
        var doctor = new Doctor { Name = "Dr A" };
        var department = new Department
        {
            Name = "Cardiology",
            Description = "Heart care",
            Doctors = new List<Doctor> { doctor }
        };

        Assert.Equal("Cardiology", department.Name);
        Assert.Equal("Heart care", department.Description);
        Assert.Single(department.Doctors);
        Assert.Same(doctor, department.Doctors.First());
    }
}
