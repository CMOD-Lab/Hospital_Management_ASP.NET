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
using DomainDoctor = ClinicManagement.Domain.Entities.Doctor;
using DomainDepartment = ClinicManagement.Domain.Entities.Department;
using DomainAppointment = ClinicManagement.Domain.Entities.Appointment;

namespace ClinicManagement.UnitTests.Web.Pages.Doctor;

/// <summary>
/// Unit tests for PatientHistoryModel page model.
/// </summary>
public class PatientHistoryModelTests
{
    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<ILogger<PatientHistoryModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly PatientHistoryModel _sut;

    public PatientHistoryModelTests()
    {
        _appointmentServiceMock = new Mock<IAppointmentService>();
        _loggerMock = new Mock<ILogger<PatientHistoryModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new PatientHistoryModel(_appointmentServiceMock.Object, _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupDoctorSession(int userId = 5)
    {
        var userTypeBytes = BitConverter.GetBytes(2);
        var userIdBytes = BitConverter.GetBytes(userId);
        if (BitConverter.IsLittleEndian) { Array.Reverse(userTypeBytes); Array.Reverse(userIdBytes); }
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
        _sut.Should().NotBeNull();
        _sut.Appointments.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotDoctor_RedirectsToLogin()
    {
        SetupNonDoctorSession();
        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenDoctor_LoadsTodaysAppointments()
    {
        // Arrange
        SetupDoctorSession(5);

        _appointmentServiceMock.Setup(s => s.GetTodaysAppointmentsByDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>
            {
                new() { AppointmentId = 1, Patient = new DomainPatient { Name = "Bob" }, Timings = "9:00 AM", Status = AppointmentStatus.Approved }
            });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Appointments.Should().HaveCount(1);
        _sut.Appointments.First().PatientName.Should().Be("Bob");
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupDoctorSession(5);
        _appointmentServiceMock.Setup(s => s.GetTodaysAppointmentsByDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
    }
}

/// <summary>
/// Unit tests for HistoryUpdateModel.
/// </summary>
public class HistoryUpdateModelTests
{
    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<ILogger<HistoryUpdateModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly HistoryUpdateModel _sut;

    public HistoryUpdateModelTests()
    {
        _appointmentServiceMock = new Mock<IAppointmentService>();
        _loggerMock = new Mock<ILogger<HistoryUpdateModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new HistoryUpdateModel(_appointmentServiceMock.Object, _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupDoctorSession(int userId = 5)
    {
        var userTypeBytes = BitConverter.GetBytes(2);
        var userIdBytes = BitConverter.GetBytes(userId);
        if (BitConverter.IsLittleEndian) { Array.Reverse(userTypeBytes); Array.Reverse(userIdBytes); }
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
        _sut.Should().NotBeNull();
        _sut.Input.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotDoctor_RedirectsToLogin()
    {
        SetupNonDoctorSession();
        var result = await _sut.OnGetAsync(1);
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenAppointmentNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupDoctorSession(5);
        _appointmentServiceMock.Setup(s => s.GetAppointmentByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainAppointment?)null);

        // Act
        var result = await _sut.OnGetAsync(99);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenAppointmentFound_LoadsInputAndReturnsPage()
    {
        // Arrange
        SetupDoctorSession(5);
        _appointmentServiceMock.Setup(s => s.GetAppointmentByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DomainAppointment
            {
                AppointmentId = 1,
                Patient = new DomainPatient { Name = "Charlie" },
                Disease = "Flu",
                Progress = "Recovering",
                Prescription = "Rest"
            });

        // Act
        var result = await _sut.OnGetAsync(1);

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Input.AppointmentId.Should().Be(1);
        _sut.Input.PatientName.Should().Be("Charlie");
        _sut.Input.Disease.Should().Be("Flu");
    }

    [Fact]
    public async Task OnPostAsync_WhenNotDoctor_RedirectsToLogin()
    {
        SetupNonDoctorSession();
        var result = await _sut.OnPostAsync();
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WhenSuccessful_SetsSuccessMessage()
    {
        // Arrange
        SetupDoctorSession(5);
        _sut.Input = new HistoryUpdateViewModel
        {
            AppointmentId = 1,
            DoctorId = 5,
            Disease = "Flu",
            Progress = "Recovering",
            Prescription = "Rest"
        };

        _appointmentServiceMock.Setup(s => s.UpdatePrescriptionAsync(5, 1, "Flu", "Recovering", "Rest", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Message.Should().Be("History updated successfully.");
    }

    [Fact]
    public async Task OnPostAsync_WhenFails_SetsErrorMessage()
    {
        // Arrange
        SetupDoctorSession(5);
        _sut.Input = new HistoryUpdateViewModel
        {
            AppointmentId = 1,
            DoctorId = 5,
            Disease = "Flu",
            Progress = "Recovering",
            Prescription = "Rest"
        };

        _appointmentServiceMock.Setup(s => s.UpdatePrescriptionAsync(5, 1, "Flu", "Recovering", "Rest", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        _sut.Message.Should().Be("Error updating history.");
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_SetsErrorMessage()
    {
        // Arrange
        SetupDoctorSession(5);
        _sut.Input = new HistoryUpdateViewModel
        {
            AppointmentId = 1,
            DoctorId = 5,
            Disease = "Flu",
            Progress = "Recovering",
            Prescription = "Rest"
        };

        _appointmentServiceMock.Setup(s => s.UpdatePrescriptionAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        _sut.Message.Should().Be("An error occurred.");
    }
}

/// <summary>
/// Unit tests for BillModel.
/// </summary>
public class BillModelTests
{
    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<ILogger<BillModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly BillModel _sut;

    public BillModelTests()
    {
        _appointmentServiceMock = new Mock<IAppointmentService>();
        _loggerMock = new Mock<ILogger<BillModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new BillModel(_appointmentServiceMock.Object, _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupDoctorSession(int userId = 5)
    {
        var userTypeBytes = BitConverter.GetBytes(2);
        var userIdBytes = BitConverter.GetBytes(userId);
        if (BitConverter.IsLittleEndian) { Array.Reverse(userTypeBytes); Array.Reverse(userIdBytes); }
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
        _sut.Should().NotBeNull();
        _sut.Appointments.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotDoctor_RedirectsToLogin()
    {
        SetupNonDoctorSession();
        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenDoctor_LoadsBillsAndReturnsPage()
    {
        // Arrange
        SetupDoctorSession(5);
        _appointmentServiceMock.Setup(s => s.GetTodaysAppointmentsByDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>
            {
                new() { AppointmentId = 1, Patient = new DomainPatient { Name = "Dave" }, BillStatus = BillStatus.Unpaid }
            });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Appointments.Should().HaveCount(1);
        _sut.Appointments.First().BillStatus.Should().Be("Unpaid");
    }

    [Fact]
    public async Task OnPostMarkPaidAsync_WhenNotDoctor_RedirectsToLogin()
    {
        SetupNonDoctorSession();
        var result = await _sut.OnPostMarkPaidAsync(1);
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnPostMarkPaidAsync_WhenDoctor_MarksBillPaidAndSetsMessage()
    {
        // Arrange
        SetupDoctorSession(5);
        _appointmentServiceMock.Setup(s => s.MarkBillPaidAsync(5, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _appointmentServiceMock.Setup(s => s.GetTodaysAppointmentsByDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>());

        // Act
        var result = await _sut.OnPostMarkPaidAsync(1);

        // Assert
        _sut.Message.Should().Be("Bill marked as paid.");
    }

    [Fact]
    public async Task OnPostMarkUnpaidAsync_WhenDoctor_MarksBillUnpaidAndSetsMessage()
    {
        // Arrange
        SetupDoctorSession(5);
        _appointmentServiceMock.Setup(s => s.MarkBillUnpaidAsync(5, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _appointmentServiceMock.Setup(s => s.GetTodaysAppointmentsByDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>());

        // Act
        var result = await _sut.OnPostMarkUnpaidAsync(1);

        // Assert
        _sut.Message.Should().Be("Bill marked as unpaid.");
    }
}

/// <summary>
/// Unit tests for PreviousHistoryModel.
/// </summary>
public class PreviousHistoryModelTests
{
    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<ILogger<PreviousHistoryModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly PreviousHistoryModel _sut;

    public PreviousHistoryModelTests()
    {
        _appointmentServiceMock = new Mock<IAppointmentService>();
        _loggerMock = new Mock<ILogger<PreviousHistoryModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new PreviousHistoryModel(_appointmentServiceMock.Object, _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupDoctorSession(int userId = 5)
    {
        var userTypeBytes = BitConverter.GetBytes(2);
        var userIdBytes = BitConverter.GetBytes(userId);
        if (BitConverter.IsLittleEndian) { Array.Reverse(userTypeBytes); Array.Reverse(userIdBytes); }
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
        _sut.Should().NotBeNull();
        _sut.History.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotDoctor_RedirectsToLogin()
    {
        SetupNonDoctorSession();
        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenDoctor_LoadsHistoryAndReturnsPage()
    {
        // Arrange
        SetupDoctorSession(5);
        _appointmentServiceMock.Setup(s => s.GetHistoryByDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>
            {
                new()
                {
                    AppointmentId = 1,
                    Patient = new DomainPatient { Name = "Eve" },
                    Disease = "Diabetes",
                    Prescription = "Metformin",
                    BillStatus = BillStatus.Paid
                }
            });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.History.Should().HaveCount(1);
        _sut.History.First().Disease.Should().Be("Diabetes");
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupDoctorSession(5);
        _appointmentServiceMock.Setup(s => s.GetHistoryByDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
    }
}
