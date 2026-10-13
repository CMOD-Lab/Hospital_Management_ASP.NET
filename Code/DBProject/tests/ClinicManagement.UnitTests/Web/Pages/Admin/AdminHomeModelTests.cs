using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.Pages.Admin;
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

namespace ClinicManagement.UnitTests.Web.Pages.Admin;

/// <summary>
/// Unit tests for AdminHomeModel page model.
/// </summary>
public class AdminHomeModelTests
{
    private readonly Mock<IPatientService> _patientServiceMock;
    private readonly Mock<IDoctorService> _doctorServiceMock;
    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<IDepartmentService> _departmentServiceMock;
    private readonly Mock<ILogger<AdminHomeModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly AdminHomeModel _sut;

    public AdminHomeModelTests()
    {
        _patientServiceMock = new Mock<IPatientService>();
        _doctorServiceMock = new Mock<IDoctorService>();
        _appointmentServiceMock = new Mock<IAppointmentService>();
        _departmentServiceMock = new Mock<IDepartmentService>();
        _loggerMock = new Mock<ILogger<AdminHomeModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new AdminHomeModel(
            _patientServiceMock.Object,
            _doctorServiceMock.Object,
            _appointmentServiceMock.Object,
            _departmentServiceMock.Object,
            _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupAdminSession()
    {
        var userTypeBytes = BitConverter.GetBytes(3);
        if (BitConverter.IsLittleEndian) Array.Reverse(userTypeBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
    }

    private void SetupNonAdminSession()
    {
        var userTypeBytes = BitConverter.GetBytes(1);
        if (BitConverter.IsLittleEndian) Array.Reverse(userTypeBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Assert
        _sut.Should().NotBeNull();
        _sut.DashboardData.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotAdmin_RedirectsToLogin()
    {
        // Arrange
        SetupNonAdminSession();

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirect = (RedirectToPageResult)result;
        redirect.PageName.Should().Be("/Account/Login");
    }

    [Fact]
    public async Task OnGetAsync_WhenAdmin_LoadsDashboardAndReturnsPage()
    {
        // Arrange
        SetupAdminSession();

        _patientServiceMock.Setup(s => s.GetAllPatientsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainPatient> { new() { PatientId = 1, Name = "P1" }, new() { PatientId = 2, Name = "P2" } });

        _doctorServiceMock.Setup(s => s.GetAllDoctorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDoctor> { new() { DoctorId = 1, Name = "D1" } });

        _departmentServiceMock.Setup(s => s.GetAllDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDepartment> { new() { DeptNo = 1, DeptName = "Cardiology" } });

        _appointmentServiceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>
            {
                new() { AppointmentId = 1, BillStatus = BillStatus.Paid, Doctor = new DomainDoctor { ChargesPerVisit = 500 } }
            });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.DashboardData.TotalPatients.Should().Be(2);
        _sut.DashboardData.TotalDoctors.Should().Be(1);
        _sut.DashboardData.TotalIncome.Should().Be(500);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPageWithEmptyData()
    {
        // Arrange
        SetupAdminSession();

        _patientServiceMock.Setup(s => s.GetAllPatientsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenAdmin_CalculatesTotalIncomeFromPaidBillsOnly()
    {
        // Arrange
        SetupAdminSession();

        _patientServiceMock.Setup(s => s.GetAllPatientsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainPatient>());

        _doctorServiceMock.Setup(s => s.GetAllDoctorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDoctor>());

        _departmentServiceMock.Setup(s => s.GetAllDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDepartment>());

        _appointmentServiceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainAppointment>
            {
                new() { AppointmentId = 1, BillStatus = BillStatus.Paid, Doctor = new DomainDoctor { ChargesPerVisit = 300 } },
                new() { AppointmentId = 2, BillStatus = BillStatus.Unpaid, Doctor = new DomainDoctor { ChargesPerVisit = 200 } },
                new() { AppointmentId = 3, BillStatus = BillStatus.Paid, Doctor = new DomainDoctor { ChargesPerVisit = 400 } }
            });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.DashboardData.TotalIncome.Should().Be(700); // Only paid: 300 + 400
    }
}
