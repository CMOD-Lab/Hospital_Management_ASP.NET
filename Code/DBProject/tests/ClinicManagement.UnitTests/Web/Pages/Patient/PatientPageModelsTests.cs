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
using DomainPatient = ClinicManagement.Domain.Entities.Patient;
using DomainDoctor = ClinicManagement.Domain.Entities.Doctor;
using DomainDepartment = ClinicManagement.Domain.Entities.Department;
using DomainAppointment = ClinicManagement.Domain.Entities.Appointment;

namespace ClinicManagement.UnitTests.Web.Pages.Patient;

/// <summary>
/// Unit tests for PatientHomeModel page model.
/// </summary>
public class PatientHomeModelTests
{
    private readonly Mock<IPatientService> _patientServiceMock;
    private readonly Mock<ILogger<PatientHomeModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly PatientHomeModel _sut;

    public PatientHomeModelTests()
    {
        _patientServiceMock = new Mock<IPatientService>();
        _loggerMock = new Mock<ILogger<PatientHomeModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new PatientHomeModel(_patientServiceMock.Object, _loggerMock.Object);

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
        _sut.PatientInfo.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotPatient_RedirectsToLogin()
    {
        SetupNonPatientSession();
        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenPatientNotFound_RedirectsToLogin()
    {
        // Arrange
        SetupPatientSession(10);
        _patientServiceMock.Setup(s => s.GetPatientByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainPatient?)null);

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenPatientFound_LoadsPatientInfoAndReturnsPage()
    {
        // Arrange
        SetupPatientSession(10);

        var patient = new DomainPatient
        {
            PatientId = 10,
            Name = "Jane Doe",
            Phone = "5559876543",
            Address = "100 Elm St",
            BirthDate = new DateTime(1995, 7, 22),
            Gender = 'F'
        };

        _patientServiceMock.Setup(s => s.GetPatientByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.PatientInfo.PatientId.Should().Be(10);
        _sut.PatientInfo.Name.Should().Be("Jane Doe");
        _sut.PatientInfo.Gender.Should().Be("F");
        _sut.PatientInfo.Age.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupPatientSession(10);
        _patientServiceMock.Setup(s => s.GetPatientByIdAsync(10, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnGetAsync_AgeCalculation_IsCorrect()
    {
        // Arrange
        SetupPatientSession(10);
        var birthDate = DateTime.Today.AddYears(-30);
        var patient = new DomainPatient
        {
            PatientId = 10,
            Name = "Test",
            BirthDate = birthDate,
            Gender = 'M'
        };

        _patientServiceMock.Setup(s => s.GetPatientByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        // Act
        await _sut.OnGetAsync();

        // Assert
        _sut.PatientInfo.Age.Should().Be(30);
    }
}

/// <summary>
/// Unit tests for ViewDoctorsModel page model.
/// </summary>
public class ViewDoctorsModelTests
{
    private readonly Mock<IDoctorService> _doctorServiceMock;
    private readonly Mock<IDepartmentService> _departmentServiceMock;
    private readonly Mock<ILogger<ViewDoctorsModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly ViewDoctorsModel _sut;

    public ViewDoctorsModelTests()
    {
        _doctorServiceMock = new Mock<IDoctorService>();
        _departmentServiceMock = new Mock<IDepartmentService>();
        _loggerMock = new Mock<ILogger<ViewDoctorsModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new ViewDoctorsModel(_doctorServiceMock.Object, _departmentServiceMock.Object, _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupPatientSession()
    {
        var userTypeBytes = BitConverter.GetBytes(1);
        if (BitConverter.IsLittleEndian) Array.Reverse(userTypeBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
    }

    private void SetupNonPatientSession()
    {
        var userTypeBytes = BitConverter.GetBytes(2);
        if (BitConverter.IsLittleEndian) Array.Reverse(userTypeBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
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
    public async Task OnGetAsync_WithNoDeptFilter_LoadsAllDoctors()
    {
        // Arrange
        SetupPatientSession();

        _departmentServiceMock.Setup(s => s.GetAllDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDepartment> { new() { DeptNo = 1, DeptName = "Cardiology" } });

        _doctorServiceMock.Setup(s => s.GetAllDoctorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDoctor>
            {
                new() { DoctorId = 1, Name = "Dr. Smith", Department = new DomainDepartment { DeptName = "Cardiology" }, ChargesPerVisit = 300 }
            });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Departments.Should().HaveCount(1);
        _sut.Doctors.Should().HaveCount(1);
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
                new() { DoctorId = 1, Name = "Dr. Heart", Department = new DomainDepartment { DeptName = "Cardiology" }, ChargesPerVisit = 500 }
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
}

/// <summary>
/// Unit tests for DoctorProfileModel page model.
/// </summary>
public class DoctorProfileModelTests
{
    private readonly Mock<IDoctorService> _doctorServiceMock;
    private readonly Mock<ILogger<DoctorProfileModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly DoctorProfileModel _sut;

    public DoctorProfileModelTests()
    {
        _doctorServiceMock = new Mock<IDoctorService>();
        _loggerMock = new Mock<ILogger<DoctorProfileModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new DoctorProfileModel(_doctorServiceMock.Object, _loggerMock.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);
        _sut.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupPatientSession()
    {
        var userTypeBytes = BitConverter.GetBytes(1);
        if (BitConverter.IsLittleEndian) Array.Reverse(userTypeBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
    }

    private void SetupNonPatientSession()
    {
        var userTypeBytes = BitConverter.GetBytes(2);
        if (BitConverter.IsLittleEndian) Array.Reverse(userTypeBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        _sut.Should().NotBeNull();
        _sut.Profile.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenNotPatient_RedirectsToLogin()
    {
        SetupNonPatientSession();
        var result = await _sut.OnGetAsync(1);
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenDoctorNotFound_ReturnsNotFound()
    {
        SetupPatientSession();
        _doctorServiceMock.Setup(s => s.GetDoctorByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainDoctor?)null);

        var result = await _sut.OnGetAsync(99);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenDoctorFound_LoadsProfileAndReturnsPage()
    {
        // Arrange
        SetupPatientSession();
        var doctor = new DomainDoctor
        {
            DoctorId = 3,
            Name = "Dr. Brown",
            Phone = "5551112222",
            Gender = 'M',
            ChargesPerVisit = 300,
            ReputeIndex = 4.5,
            PatientsTreated = 200,
            Qualification = "MBBS",
            Specialization = "Orthopedics",
            WorkExperience = 15,
            BirthDate = DateTime.Today.AddYears(-45)
        };

        _doctorServiceMock.Setup(s => s.GetDoctorByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        // Act
        var result = await _sut.OnGetAsync(3);

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Profile.DoctorId.Should().Be(3);
        _sut.Profile.Name.Should().Be("Dr. Brown");
        _sut.Profile.ChargesPerVisit.Should().Be(300);
        _sut.Profile.Age.Should().Be(45);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupPatientSession();
        _doctorServiceMock.Setup(s => s.GetDoctorByIdAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.OnGetAsync(1);
        result.Should().BeOfType<PageResult>();
    }
}
