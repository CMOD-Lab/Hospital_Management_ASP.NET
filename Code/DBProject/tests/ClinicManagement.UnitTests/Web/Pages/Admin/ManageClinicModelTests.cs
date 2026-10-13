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
using DomainDoctor = ClinicManagement.Domain.Entities.Doctor;
using DomainDepartment = ClinicManagement.Domain.Entities.Department;
using DomainStaff = ClinicManagement.Domain.Entities.OtherStaff;

namespace ClinicManagement.UnitTests.Web.Pages.Admin;

/// <summary>
/// Unit tests for ManageClinicModel page model.
/// </summary>
public class ManageClinicModelTests
{
    private readonly Mock<IDoctorService> _doctorServiceMock;
    private readonly Mock<IStaffService> _staffServiceMock;
    private readonly Mock<ILogger<ManageClinicModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly ManageClinicModel _sut;

    public ManageClinicModelTests()
    {
        _doctorServiceMock = new Mock<IDoctorService>();
        _staffServiceMock = new Mock<IStaffService>();
        _loggerMock = new Mock<ILogger<ManageClinicModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new ManageClinicModel(
            _doctorServiceMock.Object,
            _staffServiceMock.Object,
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
        _sut.Doctors.Should().NotBeNull();
        _sut.Staff.Should().NotBeNull();
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
    }

    [Fact]
    public async Task OnGetAsync_WhenAdmin_LoadsAllDoctorsAndStaff()
    {
        // Arrange
        SetupAdminSession();

        _doctorServiceMock.Setup(s => s.GetAllDoctorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDoctor>
            {
                new() { DoctorId = 1, Name = "Dr. Smith", Department = new DomainDepartment { DeptName = "Cardiology" }, Status = 1 }
            });

        _staffServiceMock.Setup(s => s.GetAllStaffAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainStaff>
            {
                new() { StaffId = 1, Name = "Nurse Jane", Designation = "Nurse" }
            });

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Doctors.Should().HaveCount(1);
        _sut.Staff.Should().HaveCount(1);
    }

    [Fact]
    public async Task OnGetAsync_WithSearchQuery_SearchesDoctorsAndStaff()
    {
        // Arrange
        SetupAdminSession();

        _doctorServiceMock.Setup(s => s.SearchDoctorsAsync("Smith", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDoctor>
            {
                new() { DoctorId = 1, Name = "Dr. Smith", Department = new DomainDepartment { DeptName = "Cardiology" }, Status = 1 }
            });

        _staffServiceMock.Setup(s => s.SearchStaffAsync("Smith", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainStaff>());

        // Act
        var result = await _sut.OnGetAsync("Smith");

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.SearchQuery.Should().Be("Smith");
        _sut.Doctors.Should().HaveCount(1);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_SetsErrorMessage()
    {
        // Arrange
        SetupAdminSession();

        _doctorServiceMock.Setup(s => s.GetAllDoctorsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Message.Should().Be("Error loading data.");
    }

    [Fact]
    public async Task OnPostDeleteDoctorAsync_WhenNotAdmin_RedirectsToLogin()
    {
        // Arrange
        SetupNonAdminSession();

        // Act
        var result = await _sut.OnPostDeleteDoctorAsync(1);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnPostDeleteDoctorAsync_WhenSuccessful_SetsSuccessMessage()
    {
        // Arrange
        SetupAdminSession();

        _doctorServiceMock.Setup(s => s.DeleteDoctorAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _doctorServiceMock.Setup(s => s.GetAllDoctorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDoctor>());

        _staffServiceMock.Setup(s => s.GetAllStaffAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainStaff>());

        // Act
        var result = await _sut.OnPostDeleteDoctorAsync(1);

        // Assert
        _sut.Message.Should().Be("Doctor removed successfully.");
    }

    [Fact]
    public async Task OnPostDeleteDoctorAsync_WhenFails_SetsErrorMessage()
    {
        // Arrange
        SetupAdminSession();

        _doctorServiceMock.Setup(s => s.DeleteDoctorAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _doctorServiceMock.Setup(s => s.GetAllDoctorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDoctor>());

        _staffServiceMock.Setup(s => s.GetAllStaffAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainStaff>());

        // Act
        var result = await _sut.OnPostDeleteDoctorAsync(1);

        // Assert
        _sut.Message.Should().Be("Error removing doctor.");
    }

    [Fact]
    public async Task OnPostDeleteStaffAsync_WhenSuccessful_SetsSuccessMessage()
    {
        // Arrange
        SetupAdminSession();

        _staffServiceMock.Setup(s => s.DeleteStaffAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _doctorServiceMock.Setup(s => s.GetAllDoctorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDoctor>());

        _staffServiceMock.Setup(s => s.GetAllStaffAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainStaff>());

        // Act
        var result = await _sut.OnPostDeleteStaffAsync(1);

        // Assert
        _sut.Message.Should().Be("Staff member removed successfully.");
    }

    [Fact]
    public async Task OnPostDeleteStaffAsync_WhenFails_SetsErrorMessage()
    {
        // Arrange
        SetupAdminSession();

        _staffServiceMock.Setup(s => s.DeleteStaffAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _doctorServiceMock.Setup(s => s.GetAllDoctorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDoctor>());

        _staffServiceMock.Setup(s => s.GetAllStaffAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainStaff>());

        // Act
        var result = await _sut.OnPostDeleteStaffAsync(1);

        // Assert
        _sut.Message.Should().Be("Error removing staff member.");
    }
}
