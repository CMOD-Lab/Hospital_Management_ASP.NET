using Xunit;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.UnitTests.Services;

/// <summary>
/// Unit tests for doctor-related domain logic.
/// </summary>
public class DoctorServiceTests
{
    [Fact]
    public void Doctor_WhenStatusIsOne_IsActive()
    {
        // Arrange
        var doctor = new Doctor
        {
            DoctorId = 1,
            Name = "Dr. Smith",
            BirthDate = new DateTime(1975, 5, 15),
            Gender = 'M',
            Status = 1
        };

        // Assert
        Assert.Equal(1, doctor.Status);
    }

    [Fact]
    public void Doctor_WhenStatusIsZero_IsInactive()
    {
        // Arrange
        var doctor = new Doctor
        {
            DoctorId = 1,
            Name = "Dr. Smith",
            BirthDate = new DateTime(1975, 5, 15),
            Gender = 'M',
            Status = 0
        };

        // Assert
        Assert.Equal(0, doctor.Status);
    }

    [Fact]
    public void Appointment_WhenApproved_HasCorrectStatus()
    {
        // Arrange
        var appointment = new Appointment
        {
            AppointmentId = 1,
            DoctorId = 1,
            PatientId = 1,
            Status = AppointmentStatus.Approved
        };

        // Assert
        Assert.Equal(AppointmentStatus.Approved, appointment.Status);
    }

    [Fact]
    public void Appointment_WhenBillPaid_HasPaidStatus()
    {
        // Arrange
        var appointment = new Appointment
        {
            AppointmentId = 1,
            BillStatus = BillStatus.Paid
        };

        // Assert
        Assert.Equal(BillStatus.Paid, appointment.BillStatus);
    }

    [Fact]
    public void UserType_PatientValue_IsOne()
    {
        Assert.Equal(1, (int)UserType.Patient);
    }

    [Fact]
    public void UserType_DoctorValue_IsTwo()
    {
        Assert.Equal(2, (int)UserType.Doctor);
    }

    [Fact]
    public void UserType_AdminValue_IsThree()
    {
        Assert.Equal(3, (int)UserType.Admin);
    }
}
