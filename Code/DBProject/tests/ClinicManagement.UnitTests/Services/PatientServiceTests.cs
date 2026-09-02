using Xunit;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.UnitTests.Services;

/// <summary>
/// Unit tests for patient-related domain entities.
/// </summary>
public class PatientServiceTests
{
    [Fact]
    public void Patient_WhenCreated_HasDefaultValues()
    {
        // Arrange & Act
        var patient = new Patient
        {
            PatientId = 1,
            Name = "John Doe",
            BirthDate = new DateTime(1990, 1, 1),
            Gender = 'M'
        };

        // Assert
        Assert.Equal(1, patient.PatientId);
        Assert.Equal("John Doe", patient.Name);
        Assert.Equal('M', patient.Gender);
        Assert.NotNull(patient.Appointments);
    }

    [Fact]
    public void Doctor_WhenCreated_HasActiveStatus()
    {
        // Arrange & Act
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
        Assert.Equal("Dr. Smith", doctor.Name);
    }

    [Fact]
    public void Appointment_WhenCreated_HasPendingStatus()
    {
        // Arrange & Act
        var appointment = new Appointment
        {
            AppointmentId = 1,
            DoctorId = 1,
            PatientId = 1,
            Status = AppointmentStatus.Pending
        };

        // Assert
        Assert.Equal(AppointmentStatus.Pending, appointment.Status);
        Assert.Equal(BillStatus.Unpaid, appointment.BillStatus);
    }

    [Fact]
    public void Department_WhenCreated_HasEmptyDoctorsList()
    {
        // Arrange & Act
        var department = new Department
        {
            DeptNo = 1,
            DeptName = "Cardiology"
        };

        // Assert
        Assert.Equal("Cardiology", department.DeptName);
        Assert.NotNull(department.Doctors);
        Assert.Empty(department.Doctors);
    }

    [Fact]
    public void LoginEntry_WhenCreated_HasRequiredFields()
    {
        // Arrange & Act
        var loginEntry = new LoginEntry
        {
            LoginId = 1,
            Email = "test@clinic.com",
            Password = "password123",
            Type = (int)UserType.Patient
        };

        // Assert
        Assert.Equal("test@clinic.com", loginEntry.Email);
        Assert.Equal((int)UserType.Patient, loginEntry.Type);
    }
}
