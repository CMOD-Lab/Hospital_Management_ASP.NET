using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ClinicManagement.UnitTests.Services;

/// <summary>Unit tests for domain entity validation.</summary>
public class DomainEntityTests
{
    [Fact]
    public void Patient_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var patient = new Patient();

        // Assert
        patient.IsActive.Should().BeTrue();
        patient.Name.Should().BeEmpty();
        patient.Email.Should().BeEmpty();
    }

    [Fact]
    public void Doctor_ShouldHaveDefaultStatus()
    {
        // Arrange & Act
        var doctor = new Doctor();

        // Assert
        doctor.Status.Should().Be(1);
        doctor.Name.Should().BeEmpty();
    }

    [Fact]
    public void UserType_ShouldHaveCorrectValues()
    {
        // Assert
        ((int)UserType.Patient).Should().Be(1);
        ((int)UserType.Doctor).Should().Be(2);
        ((int)UserType.Admin).Should().Be(3);
    }

    [Fact]
    public void Appointment_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var appointment = new Appointment();

        // Assert
        appointment.IsPaid.Should().BeFalse();
        appointment.FeedbackGiven.Should().BeFalse();
        appointment.DoctorName.Should().BeEmpty();
    }

    [Fact]
    public void Department_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var dept = new Department { DeptNo = 1, DeptName = "Cardiology" };

        // Assert
        dept.DeptNo.Should().Be(1);
        dept.DeptName.Should().Be("Cardiology");
    }
}
