using ClinicManagementSystem.Domain.Entities;
using Xunit;

namespace ClinicManagementSystem.Domain.Entities;

public class PatientTests
{
    [Fact]
    public void Constructor_InitializesCollectionsAndDefaults()
    {
        var patient = new Patient();

        Assert.Equal(string.Empty, patient.Name);
        Assert.Equal(string.Empty, patient.Email);
        Assert.Equal(string.Empty, patient.PhoneNumber);
        Assert.Equal(string.Empty, patient.Gender);
        Assert.Equal(string.Empty, patient.Address);
        Assert.NotNull(patient.Appointments);
        Assert.NotNull(patient.FeedbackEntries);
        Assert.Empty(patient.Appointments);
        Assert.Empty(patient.FeedbackEntries);
    }

    [Fact]
    public void Properties_CanBeAssigned()
    {
        var birthDate = new DateTime(1990, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        var appointment = new Appointment { Name = "Checkup" };
        var feedback = new Feedback { Comments = "Great", Rating = 5 };

        var patient = new Patient
        {
            Name = "Jane",
            Email = "jane@example.com",
            PhoneNumber = "123",
            BirthDate = birthDate,
            Gender = "F",
            Address = "Road",
            Appointments = new List<Appointment> { appointment },
            FeedbackEntries = new List<Feedback> { feedback }
        };

        Assert.Equal("Jane", patient.Name);
        Assert.Equal("jane@example.com", patient.Email);
        Assert.Equal(birthDate, patient.BirthDate);
        Assert.Single(patient.Appointments);
        Assert.Single(patient.FeedbackEntries);
    }
}
