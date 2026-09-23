using ClinicManagementSystem.Domain.Entities;
using Xunit;

namespace ClinicManagementSystem.Domain.Entities;

public class FeedbackTests
{
    [Fact]
    public void Constructor_InitializesDefaults()
    {
        var feedback = new Feedback();

        Assert.Equal(string.Empty, feedback.Name);
        Assert.Null(feedback.Description);
        Assert.Equal(0, feedback.PatientId);
        Assert.Equal(0, feedback.DoctorId);
        Assert.Equal(0, feedback.Rating);
        Assert.Equal(string.Empty, feedback.Comments);
    }

    [Fact]
    public void Properties_CanBeAssigned()
    {
        var patient = new Patient { Id = 1, Name = "Pat" };
        var doctor = new Doctor { Id = 2, Name = "Doc" };
        var feedback = new Feedback
        {
            Name = "Post visit",
            Description = "desc",
            PatientId = 1,
            Patient = patient,
            DoctorId = 2,
            Doctor = doctor,
            Rating = 4,
            Comments = "Helpful"
        };

        Assert.Same(patient, feedback.Patient);
        Assert.Same(doctor, feedback.Doctor);
        Assert.Equal(4, feedback.Rating);
        Assert.Equal("Helpful", feedback.Comments);
    }
}
