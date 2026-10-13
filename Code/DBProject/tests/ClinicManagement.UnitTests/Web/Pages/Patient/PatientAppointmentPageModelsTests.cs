using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.Pages.Patient;
using ClinicManagement.Web.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using DomainDoctor = ClinicManagement.Domain.Entities.Doctor;
using DomainDepartment = ClinicManagement.Domain.Entities.Department;
using DomainAppointment = ClinicManagement.Domain.Entities.Appointment;

namespace ClinicManagement.UnitTests.Web.Pages.Patient;

/// <summary>
/// Unit tests for TakeAppointmentModel page model.
/// </summary>
public class TakeAppointmentModelTests
{
    private readonly Mock<IDoctorService> _doctorServiceMock;
    private readonly Mock<IDepartmentService> _departmentServiceMock;
    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<ILogger<TakeAppointmentModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly TakeAppointmentModel _sut;

    public TakeAppointmentModelTests()
    {
        _doctorServiceMock = new Mock<IDoctorService>();
        _departmentServiceMock = new Mock<IDepartmentService>();
        _appointmentServiceMock = new Mock<IAppointmentService>();
        _loggerMock = new Mock<ILogger<TakeAppointmentModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new TakeAppointmentModel(
            _doctorServiceMock.Object,
            _departmentServiceMock.Object,
            _appointmentServiceMock.Object,
            _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupPatientSession(int userId = 10)
    {
        var userTypeBytes = BitConverter.GetBytes(1);
        var userIdBytes = BitConverter.GetBytes(userId);
        if (BitConverter.IsLittleEndian) { Array.Reverse(userTypeBytes); Array.Reverse(userIdBytes); }
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(true);
    }

    private void SetupNonPatientSession()
    {
        var userTypeBytes = BitConverter.GetBytes(2);
        if (BitConverter.IsLittleEndian) Array.Reverse(userTypeBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        byte[]? userIdBytes = null;
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(false);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        _sut.Should().NotBeNull();
        _sut.Departments.Should().NotBeNull();
        _sut.Doctors.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotPatient_RedirectsToLogin()
    {
        SetupNonPatientSession();
        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WithNoDeptFilter_LoadsDepartmentsOnly()
    {
        // Arrange
        SetupPatientSession();
        _departmentServiceMock.Setup(s => s.GetAllDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDepartment> { new() { DeptNo = 1, DeptName = "Cardiology" } });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Departments.Should().HaveCount(1);
        _sut.Doctors.Should().BeEmpty();
    }

    [Fact]
    public async Task OnGetAsync_WithDeptFilter_LoadsDoctorsByDepartment()
    {
        // Arrange
        SetupPatientSession();
        _departmentServiceMock.Setup(s => s.GetAllDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDepartment> { new() { DeptNo = 1, DeptName = "Cardiology" } });

        _doctorServiceMock.Setup(s => s.GetDoctorsByDepartmentAsync("Cardiology", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDoctor>
            {
                new() { DoctorId = 1, Name = "Dr. Heart", ChargesPerVisit = 500 }
            });

        // Act
        var result = await _sut.OnGetAsync("Cardiology");

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.SelectedDepartment.Should().Be("Cardiology");
        _sut.Doctors.Should().HaveCount(1);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupPatientSession();
        _departmentServiceMock.Setup(s => s.GetAllDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WhenNotPatient_RedirectsToLogin()
    {
        SetupNonPatientSession();
        var result = await _sut.OnPostAsync(1);
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WhenBookingSucceeds_SetsSuccessMessage()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.BookAppointmentAsync(3, 10, 0, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _departmentServiceMock.Setup(s => s.GetAllDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDepartment>());

        // Act
        var result = await _sut.OnPostAsync(3);

        // Assert
        _sut.Message.Should().Be("Appointment request sent successfully! Please wait for doctor approval.");
    }

    [Fact]
    public async Task OnPostAsync_WhenBookingFails_SetsErrorMessage()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.BookAppointmentAsync(3, 10, 0, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _departmentServiceMock.Setup(s => s.GetAllDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDepartment>());

        // Act
        var result = await _sut.OnPostAsync(3);

        // Assert
        _sut.Message.Should().Be("Failed to book appointment. Please try again.");
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_SetsErrorMessage()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.BookAppointmentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        _departmentServiceMock.Setup(s => s.GetAllDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDepartment>());

        // Act
        var result = await _sut.OnPostAsync(3);

        // Assert
        _sut.Message.Should().Be("An error occurred while booking the appointment.");
    }
}

/// <summary>
/// Unit tests for CurrentAppointmentModel.
/// </summary>
public class CurrentAppointmentModelTests
{
    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<ILogger<CurrentAppointmentModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly CurrentAppointmentModel _sut;

    public CurrentAppointmentModelTests()
    {
        _appointmentServiceMock = new Mock<IAppointmentService>();
        _loggerMock = new Mock<ILogger<CurrentAppointmentModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new CurrentAppointmentModel(_appointmentServiceMock.Object, _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupPatientSession(int userId = 10)
    {
        var userTypeBytes = BitConverter.GetBytes(1);
        var userIdBytes = BitConverter.GetBytes(userId);
        if (BitConverter.IsLittleEndian) { Array.Reverse(userTypeBytes); Array.Reverse(userIdBytes); }
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(true);
    }

    private void SetupNonPatientSession()
    {
        var userTypeBytes = BitConverter.GetBytes(2);
        if (BitConverter.IsLittleEndian) Array.Reverse(userTypeBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        byte[]? userIdBytes = null;
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(false);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        _sut.Should().NotBeNull();
        _sut.AppointmentInfo.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotPatient_RedirectsToLogin()
    {
        SetupNonPatientSession();
        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenNoCurrentAppointment_SetsHasAppointmentFalse()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.GetCurrentAppointmentByPatientAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainAppointment?)null);

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.AppointmentInfo.HasAppointment.Should().BeFalse();
        _sut.AppointmentInfo.DoctorName.Should().BeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenCurrentAppointmentExists_SetsHasAppointmentTrue()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.GetCurrentAppointmentByPatientAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DomainAppointment
            {
                AppointmentId = 1,
                Doctor = new DomainDoctor { Name = "Dr. Green" },
                Timings = "10:00 AM"
            });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.AppointmentInfo.HasAppointment.Should().BeTrue();
        _sut.AppointmentInfo.DoctorName.Should().Be("Dr. Green");
        _sut.AppointmentInfo.Timings.Should().Be("10:00 AM");
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.GetCurrentAppointmentByPatientAsync(10, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
    }
}

/// <summary>
/// Unit tests for BillsHistoryModel.
/// </summary>
public class BillsHistoryModelTests
{
    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<ILogger<BillsHistoryModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly BillsHistoryModel _sut;

    public BillsHistoryModelTests()
    {
        _appointmentServiceMock = new Mock<IAppointmentService>();
        _loggerMock = new Mock<ILogger<BillsHistoryModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new BillsHistoryModel(_appointmentServiceMock.Object, _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupPatientSession(int userId = 10)
    {
        var userTypeBytes = BitConverter.GetBytes(1);
        var userIdBytes = BitConverter.GetBytes(userId);
        if (BitConverter.IsLittleEndian) { Array.Reverse(userTypeBytes); Array.Reverse(userIdBytes); }
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(true);
    }

    private void SetupNonPatientSession()
    {
        var userTypeBytes = BitConverter.GetBytes(2);
        if (BitConverter.IsLittleEndian) Array.Reverse(userTypeBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        byte[]? userIdBytes = null;
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(false);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        _sut.Should().NotBeNull();
        _sut.Bills.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotPatient_RedirectsToLogin()
    {
        SetupNonPatientSession();
        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenPatient_LoadsBillsAndReturnsPage()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.GetBillHistoryByPatientAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>
            {
                new() { AppointmentId = 1, Doctor = new DomainDoctor { Name = "Dr. Smith" }, BillStatus = BillStatus.Paid }
            });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Bills.Should().HaveCount(1);
        _sut.Bills.First().BillStatus.Should().Be("Paid");
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.GetBillHistoryByPatientAsync(10, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
    }
}

/// <summary>
/// Unit tests for TreatmentHistoryModel.
/// </summary>
public class TreatmentHistoryModelTests
{
    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<ILogger<TreatmentHistoryModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly TreatmentHistoryModel _sut;

    public TreatmentHistoryModelTests()
    {
        _appointmentServiceMock = new Mock<IAppointmentService>();
        _loggerMock = new Mock<ILogger<TreatmentHistoryModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new TreatmentHistoryModel(_appointmentServiceMock.Object, _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupPatientSession(int userId = 10)
    {
        var userTypeBytes = BitConverter.GetBytes(1);
        var userIdBytes = BitConverter.GetBytes(userId);
        if (BitConverter.IsLittleEndian) { Array.Reverse(userTypeBytes); Array.Reverse(userIdBytes); }
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(true);
    }

    private void SetupNonPatientSession()
    {
        var userTypeBytes = BitConverter.GetBytes(2);
        if (BitConverter.IsLittleEndian) Array.Reverse(userTypeBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        byte[]? userIdBytes = null;
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(false);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        _sut.Should().NotBeNull();
        _sut.Treatments.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotPatient_RedirectsToLogin()
    {
        SetupNonPatientSession();
        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenPatient_LoadsTreatmentsAndReturnsPage()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.GetTreatmentHistoryByPatientAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>
            {
                new()
                {
                    AppointmentId = 1,
                    Doctor = new DomainDoctor { Name = "Dr. Smith" },
                    Disease = "Flu",
                    Progress = "Recovered",
                    Prescription = "Rest"
                }
            });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Treatments.Should().HaveCount(1);
        _sut.Treatments.First().Disease.Should().Be("Flu");
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.GetTreatmentHistoryByPatientAsync(10, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
    }
}

/// <summary>
/// Unit tests for PatientNotificationsModel.
/// </summary>
public class PatientNotificationsModelTests
{
    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<ILogger<PatientNotificationsModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly PatientNotificationsModel _sut;

    public PatientNotificationsModelTests()
    {
        _appointmentServiceMock = new Mock<IAppointmentService>();
        _loggerMock = new Mock<ILogger<PatientNotificationsModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new PatientNotificationsModel(_appointmentServiceMock.Object, _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupPatientSession(int userId = 10)
    {
        var userTypeBytes = BitConverter.GetBytes(1);
        var userIdBytes = BitConverter.GetBytes(userId);
        if (BitConverter.IsLittleEndian) { Array.Reverse(userTypeBytes); Array.Reverse(userIdBytes); }
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(true);
    }

    private void SetupNonPatientSession()
    {
        var userTypeBytes = BitConverter.GetBytes(2);
        if (BitConverter.IsLittleEndian) Array.Reverse(userTypeBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        byte[]? userIdBytes = null;
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(false);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        _sut.Should().NotBeNull();
        _sut.NotificationInfo.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotPatient_RedirectsToLogin()
    {
        SetupNonPatientSession();
        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenNoNotification_SetsHasNotificationFalse()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.GetNotificationByPatientAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainAppointment?)null);

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.NotificationInfo.HasNotification.Should().BeFalse();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotificationExists_SetsHasNotificationTrue()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.GetNotificationByPatientAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DomainAppointment
            {
                AppointmentId = 1,
                Doctor = new DomainDoctor { Name = "Dr. White" },
                Timings = "2:00 PM"
            });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.NotificationInfo.HasNotification.Should().BeTrue();
        _sut.NotificationInfo.DoctorName.Should().Be("Dr. White");
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.GetNotificationByPatientAsync(10, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
    }
}

/// <summary>
/// Unit tests for PatientFeedbackModel.
/// </summary>
public class PatientFeedbackModelTests
{
    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<ILogger<PatientFeedbackModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly PatientFeedbackModel _sut;

    public PatientFeedbackModelTests()
    {
        _appointmentServiceMock = new Mock<IAppointmentService>();
        _loggerMock = new Mock<ILogger<PatientFeedbackModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new PatientFeedbackModel(_appointmentServiceMock.Object, _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupPatientSession(int userId = 10)
    {
        var userTypeBytes = BitConverter.GetBytes(1);
        var userIdBytes = BitConverter.GetBytes(userId);
        if (BitConverter.IsLittleEndian) { Array.Reverse(userTypeBytes); Array.Reverse(userIdBytes); }
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(true);
    }

    private void SetupNonPatientSession()
    {
        var userTypeBytes = BitConverter.GetBytes(2);
        if (BitConverter.IsLittleEndian) Array.Reverse(userTypeBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        byte[]? userIdBytes = null;
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(false);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        _sut.Should().NotBeNull();
        _sut.FeedbackInfo.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotPatient_RedirectsToLogin()
    {
        SetupNonPatientSession();
        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenNoPendingFeedback_SetsHasPendingFeedbackFalse()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.GetPendingFeedbackByPatientAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainAppointment?)null);

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.FeedbackInfo.HasPendingFeedback.Should().BeFalse();
        _sut.FeedbackInfo.AppointmentId.Should().Be(0);
    }

    [Fact]
    public async Task OnGetAsync_WhenPendingFeedbackExists_SetsHasPendingFeedbackTrue()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.GetPendingFeedbackByPatientAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DomainAppointment
            {
                AppointmentId = 5,
                Doctor = new DomainDoctor { Name = "Dr. Black" },
                Timings = "3:30 PM"
            });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.FeedbackInfo.HasPendingFeedback.Should().BeTrue();
        _sut.FeedbackInfo.AppointmentId.Should().Be(5);
        _sut.FeedbackInfo.DoctorName.Should().Be("Dr. Black");
    }

    [Fact]
    public async Task OnPostAsync_WhenNotPatient_RedirectsToLogin()
    {
        SetupNonPatientSession();
        var result = await _sut.OnPostAsync(1);
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WhenFeedbackStoredSuccessfully_SetsThankYouMessage()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.StoreFeedbackAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.OnPostAsync(5);

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.FeedbackInfo.Message.Should().Be("Thank you for your feedback!");
    }

    [Fact]
    public async Task OnPostAsync_WhenFeedbackStoreFails_SetsErrorMessage()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.StoreFeedbackAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.OnPostAsync(5);

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.FeedbackInfo.Message.Should().Be("Error submitting feedback.");
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_SetsErrorMessage()
    {
        // Arrange
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.StoreFeedbackAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.OnPostAsync(5);

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.FeedbackInfo.Message.Should().Be("An error occurred.");
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupPatientSession(10);
        _appointmentServiceMock.Setup(s => s.GetPendingFeedbackByPatientAsync(10, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
    }
}
