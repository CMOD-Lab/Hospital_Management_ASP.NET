using ClinicManagementSystem.Domain.Entities;
using Xunit;

namespace ClinicManagementSystem.Domain.Entities;

public class AppointmentTests
{
    [Fact]
    public void Constructor_InitializesDefaults()
    {
        var appointment = new Appointment();

        Assert.Equal(string.Empty, appointment.Name);
        Assert.Null(appointment.Description);
        Assert.Equal(0, appointment.PatientId);
        Assert.Equal(0, appointment.DoctorId);
        Assert.Equal(default, appointment.ScheduledAt);
        Assert.Equal(string.Empty, appointment.Status);
        Assert.Null(appointment.Prescription);
        Assert.Null(appointment.ProgressNotes);
        Assert.Null(appointment.Disease);
        Assert.Null(appointment.Patient);
        Assert.Null(appointment.Doctor);
        Assert.Null(appointment.Bill);
    }

    [Fact]
    public void Properties_CanBeAssigned()
    {
        var patient = new Patient { Id = 1, Name = "Pat" };
        var doctor = new Doctor { Id = 2, Name = "Doc" };
        var bill = new Bill { Id = 3, Name = "Consultation" };
        var scheduled = DateTime.UtcNow;

        var appointment = new Appointment
        {
            Name = "Checkup",
            Description = "desc",
            PatientId = 1,
            Patient = patient,
            DoctorId = 2,
            Doctor = doctor,
            ScheduledAt = scheduled,
            Status = "Pending",
            Prescription = "Rx",
            ProgressNotes = "Notes",
            Disease = "Cold",
            Bill = bill
        };

        Assert.Equal("Checkup", appointment.Name);
        Assert.Same(patient, appointment.Patient);
        Assert.Same(doctor, appointment.Doctor);
        Assert.Same(bill, appointment.Bill);
        Assert.Equal(scheduled, appointment.ScheduledAt);
        Assert.Equal("Pending", appointment.Status);
    }
}
