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
using DomainDoctor = ClinicManagement.Domain.Entities.Doctor;
using DomainDepartment = ClinicManagement.Domain.Entities.Department;

namespace ClinicManagement.UnitTests.Web.Pages.Doctor;

/// <summary>
/// Unit tests for DoctorHomeModel page model.
/// </summary>
public class DoctorHomeModelTests
{
    private readonly Mock<IDoctorService> _doctorServiceMock;
    private readonly Mock<ILogger<DoctorHomeModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly DoctorHomeModel _sut;

    public DoctorHomeModelTests()
    {
        _doctorServiceMock = new Mock<IDoctorService>();
        _loggerMock = new Mock<ILogger<DoctorHomeModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new DoctorHomeModel(_doctorServiceMock.Object, _loggerMock.Object);

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
        _sut.DoctorInfo.Should().NotBeNull();
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
    public async Task OnGetAsync_WhenDoctorNotFound_RedirectsToLogin()
    {
        // Arrange
        SetupDoctorSession(5);

        _doctorServiceMock.Setup(s => s.GetDoctorByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainDoctor?)null);

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenDoctorFound_LoadsDoctorInfoAndReturnsPage()
    {
        // Arrange
        SetupDoctorSession(5);

        var doctor = new DomainDoctor
        {
            DoctorId = 5,
            Name = "Dr. Smith",
            Phone = "5551234567",
            Address = "123 Main St",
            Gender = 'M',
            Department = new DomainDepartment { DeptName = "Cardiology" },
            ChargesPerVisit = 300,
            WorkExperience = 10,
            Qualification = "MBBS",
            Specialization = "Heart"
        };

        _doctorServiceMock.Setup(s => s.GetDoctorByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.DoctorInfo.DoctorId.Should().Be(5);
        _sut.DoctorInfo.Name.Should().Be("Dr. Smith");
        _sut.DoctorInfo.DepartmentName.Should().Be("Cardiology");
        _sut.DoctorInfo.ChargesPerVisit.Should().Be(300);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        // Arrange
        SetupDoctorSession(5);

        _doctorServiceMock.Setup(s => s.GetDoctorByIdAsync(5, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenDoctorHasNoDepartment_SetsDepartmentNameToEmpty()
    {
        // Arrange
        SetupDoctorSession(5);

        var doctor = new DomainDoctor
        {
            DoctorId = 5,
            Name = "Dr. No Dept",
            Gender = 'F',
            Department = null
        };

        _doctorServiceMock.Setup(s => s.GetDoctorByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.DoctorInfo.DepartmentName.Should().Be(string.Empty);
    }
}
