using CareTrack.Application.Services;
using Xunit;
using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CareTrack.UnitTests.Services;

/// <summary>
/// Unit tests for the DoctorService.
/// </summary>
public class DoctorServiceTests
{
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly Mock<ILogger<DoctorService>> _loggerMock;
    private readonly DoctorService _doctorService;

    public DoctorServiceTests()
    {
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _loggerMock = new Mock<ILogger<DoctorService>>();
        _doctorService = new DoctorService(
            _doctorRepositoryMock.Object,
            _appointmentRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetDoctorInfoAsync_WithValidId_ReturnsDoctor()
    {
        // Arrange
        var doctor = new Doctor { DoctorId = 1, Name = "Dr. Smith", Status = 1 };
        _doctorRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(doctor);

        // Act
        var result = await _doctorService.GetDoctorInfoAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Dr. Smith");
    }

    [Fact]
    public async Task GetDoctorInfoAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _doctorRepositoryMock.Setup(r => r.GetByIdAsync(999, default))
            .ReturnsAsync((Doctor?)null);

        // Act
        var result = await _doctorService.GetDoctorInfoAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ApproveAppointmentAsync_WithValidAppointment_ReturnsTrue()
    {
        // Arrange
        var appointment = new Appointment { AppointId = 1, AppointmentStatus = 2, DoctorId = 1 };
        _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(appointment);
        _appointmentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _doctorService.ApproveAppointmentAsync(1);

        // Assert
        result.Should().BeTrue();
        appointment.AppointmentStatus.Should().Be(1); // Approved
    }

    [Fact]
    public async Task ApproveAppointmentAsync_WithInvalidAppointment_ReturnsFalse()
    {
        // Arrange
        _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(999, default))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _doctorService.ApproveAppointmentAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetPendingAppointmentsAsync_ReturnsOnlyPendingAppointments()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new Appointment { AppointId = 1, AppointmentStatus = 2, DoctorId = 1 },
            new Appointment { AppointId = 2, AppointmentStatus = 2, DoctorId = 1 }
        };
        _appointmentRepositoryMock.Setup(r => r.GetPendingByDoctorIdAsync(1, default))
            .ReturnsAsync(appointments);

        // Act
        var result = await _doctorService.GetPendingAppointmentsAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.All(a => a.AppointmentStatus == 2).Should().BeTrue();
    }
}
