using ClinicManagementSystem.Domain.Entities;
using Xunit;

namespace ClinicManagementSystem.Domain.Entities;

public class DoctorTests
{
    [Fact]
    public void Constructor_InitializesDefaults()
    {
        var doctor = new Doctor();

        Assert.Equal(string.Empty, doctor.Name);
        Assert.Equal(string.Empty, doctor.Email);
        Assert.Equal(string.Empty, doctor.PhoneNumber);
        Assert.Equal(string.Empty, doctor.Gender);
        Assert.Equal(string.Empty, doctor.Qualification);
        Assert.Equal(string.Empty, doctor.Specialization);
        Assert.Equal(string.Empty, doctor.Address);
        Assert.NotNull(doctor.Appointments);
        Assert.Empty(doctor.Appointments);
    }

    [Fact]
    public void Properties_CanBeAssigned()
    {
        var department = new Department { Id = 9, Name = "Neuro" };
        var appointment = new Appointment { Name = "Consultation" };
        var doctor = new Doctor
        {
            Name = "Dr B",
            Email = "doctor@example.com",
            PhoneNumber = "999",
            Gender = "M",
            Qualification = "MBBS",
            Specialization = "Neuro",
            Address = "Street",
            ChargesPerVisit = 125.5m,
            ExperienceYears = 10,
            DepartmentId = 9,
            Department = department,
            Appointments = new List<Appointment> { appointment }
        };

        Assert.Equal(125.5m, doctor.ChargesPerVisit);
        Assert.Equal(10, doctor.ExperienceYears);
        Assert.Equal(9, doctor.DepartmentId);
        Assert.Same(department, doctor.Department);
        Assert.Single(doctor.Appointments);
    }
}
