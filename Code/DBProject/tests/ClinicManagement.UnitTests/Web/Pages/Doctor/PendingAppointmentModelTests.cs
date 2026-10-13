using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.Pages.Doctor;
using ClinicManagement.Web.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using DomainPatient = ClinicManagement.Domain.Entities.Patient;
using DomainAppointment = ClinicManagement.Domain.Entities.Appointment;

namespace ClinicManagement.UnitTests.Web.Pages.Doctor;

/// <summary>
/// Unit tests for PendingAppointmentModel page model.
/// </summary>
public class PendingAppointmentModelTests
{
    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<ILogger<PendingAppointmentModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly PendingAppointmentModel _sut;

    public PendingAppointmentModelTests()
    {
        _appointmentServiceMock = new Mock<IAppointmentService>();
        _loggerMock = new Mock<ILogger<PendingAppointmentModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new PendingAppointmentModel(_appointmentServiceMock.Object, _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupDoctorSession(int userId = 5)
    {
        var userTypeBytes = BitConverter.GetBytes(2);
        var userIdBytes = BitConverter.GetBytes(userId);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(userTypeBytes);
            Array.Reverse(userIdBytes);
        }
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(true);
    }

    private void SetupNonDoctorSession()
    {
        var userTypeBytes = BitConverter.GetBytes(1);
        if (BitConverter.IsLittleEndian) Array.Reverse(userTypeBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        byte[]? userIdBytes = null;
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(false);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Assert
        _sut.Should().NotBeNull();
        _sut.Appointments.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotDoctor_RedirectsToLogin()
    {
        // Arrange
        SetupNonDoctorSession();

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenDoctor_LoadsPendingAppointments()
    {
        // Arrange
        SetupDoctorSession(5);

        _appointmentServiceMock.Setup(s => s.GetPendingAppointmentsByDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>
            {
                new()
                {
                    AppointmentId = 1,
                    Patient = new DomainPatient { Name = "Alice" },
                    AppointmentDate = DateTime.Today,
                    Timings = "10:00 AM",
                    Status = AppointmentStatus.Pending
                }
            });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Appointments.Should().HaveCount(1);
        _sut.Appointments.First().PatientName.Should().Be("Alice");
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_SetsErrorMessage()
    {
        // Arrange
        SetupDoctorSession(5);

        _appointmentServiceMock.Setup(s => s.GetPendingAppointmentsByDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Message.Should().Be("Error loading appointments.");
    }

    [Fact]
    public async Task OnPostApproveAsync_WhenNotDoctor_RedirectsToLogin()
    {
        // Arrange
        SetupNonDoctorSession();

        // Act
        var result = await _sut.OnPostApproveAsync(1);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnPostApproveAsync_WhenSuccessful_SetsApprovedMessage()
    {
        // Arrange
        SetupDoctorSession(5);

        _appointmentServiceMock.Setup(s => s.ApproveAppointmentAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _appointmentServiceMock.Setup(s => s.GetPendingAppointmentsByDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>());

        // Act
        var result = await _sut.OnPostApproveAsync(1);

        // Assert
        _sut.Message.Should().Be("Appointment approved.");
    }

    [Fact]
    public async Task OnPostApproveAsync_WhenFails_SetsErrorMessage()
    {
        // Arrange
        SetupDoctorSession(5);

        _appointmentServiceMock.Setup(s => s.ApproveAppointmentAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _appointmentServiceMock.Setup(s => s.GetPendingAppointmentsByDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>());

        // Act
        var result = await _sut.OnPostApproveAsync(1);

        // Assert
        _sut.Message.Should().Be("Error approving appointment.");
    }

    [Fact]
    public async Task OnPostRejectAsync_WhenSuccessful_SetsRejectedMessage()
    {
        // Arrange
        SetupDoctorSession(5);

        _appointmentServiceMock.Setup(s => s.DeleteAppointmentAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _appointmentServiceMock.Setup(s => s.GetPendingAppointmentsByDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>());

        // Act
        var result = await _sut.OnPostRejectAsync(1);

        // Assert
        _sut.Message.Should().Be("Appointment rejected.");
    }

    [Fact]
    public async Task OnPostRejectAsync_WhenFails_SetsErrorMessage()
    {
        // Arrange
        SetupDoctorSession(5);

        _appointmentServiceMock.Setup(s => s.DeleteAppointmentAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _appointmentServiceMock.Setup(s => s.GetPendingAppointmentsByDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>());

        // Act
        var result = await _sut.OnPostRejectAsync(1);

        // Assert
        _sut.Message.Should().Be("Error rejecting appointment.");
    }
}
