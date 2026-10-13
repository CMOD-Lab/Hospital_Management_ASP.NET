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

namespace ClinicManagement.UnitTests.Web.Pages.Admin;

/// <summary>
/// Unit tests for DoctorRegistrationFormModel page model.
/// </summary>
public class DoctorRegistrationFormModelTests
{
    private readonly Mock<IDoctorService> _doctorServiceMock;
    private readonly Mock<IDepartmentService> _departmentServiceMock;
    private readonly Mock<ILogger<DoctorRegistrationFormModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly DoctorRegistrationFormModel _sut;

    public DoctorRegistrationFormModelTests()
    {
        _doctorServiceMock = new Mock<IDoctorService>();
        _departmentServiceMock = new Mock<IDepartmentService>();
        _loggerMock = new Mock<ILogger<DoctorRegistrationFormModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new DoctorRegistrationFormModel(
            _doctorServiceMock.Object,
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

    private void SetupDepartments()
    {
        _departmentServiceMock.Setup(s => s.GetAllDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainDepartment>
            {
                new() { DeptNo = 1, DeptName = "Cardiology" },
                new() { DeptNo = 2, DeptName = "Neurology" }
            });
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Assert
        _sut.Should().NotBeNull();
        _sut.Input.Should().NotBeNull();
        _sut.Departments.Should().NotBeNull();
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
    public async Task OnGetAsync_WhenAdmin_LoadsDepartmentsAndReturnsPage()
    {
        // Arrange
        SetupAdminSession();
        SetupDepartments();

        // Act
        var result = await _sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Departments.Should().HaveCount(2);
    }

    [Fact]
    public async Task OnPostAsync_WhenNotAdmin_RedirectsToLogin()
    {
        // Arrange
        SetupNonAdminSession();
        SetupDepartments();

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidModelState_ReturnsPage()
    {
        // Arrange
        SetupAdminSession();
        SetupDepartments();
        _sut.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WhenEmailAlreadyExists_SetsErrorMessage()
    {
        // Arrange
        SetupAdminSession();
        SetupDepartments();

        _sut.Input = new DoctorRegistrationViewModel
        {
            Name = "Dr. Alice",
            Email = "existing@clinic.com",
            Password = "Doc@123",
            BirthDate = "1980-05-20",
            DeptNo = 1,
            Gender = "F",
            WorkExperience = 10,
            Salary = 80000,
            ChargesPerVisit = 500
        };

        _doctorServiceMock.Setup(s => s.EmailExistsAsync("existing@clinic.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.ErrorMessage.Should().Be("A doctor with this email already exists.");
    }

    [Fact]
    public async Task OnPostAsync_WithValidInput_RegistersDoctorAndSetsSuccessMessage()
    {
        // Arrange
        SetupAdminSession();
        SetupDepartments();

        _sut.Input = new DoctorRegistrationViewModel
        {
            Name = "Dr. New",
            Email = "new@clinic.com",
            Password = "Doc@123",
            BirthDate = "1985-01-15",
            DeptNo = 1,
            Gender = "M",
            WorkExperience = 5,
            Salary = 70000,
            ChargesPerVisit = 400
        };

        _doctorServiceMock.Setup(s => s.EmailExistsAsync("new@clinic.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _doctorServiceMock.Setup(s => s.AddDoctorAsync(It.IsAny<DomainDoctor>(), "new@clinic.com", "Doc@123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.SuccessMessage.Should().Be("Doctor registered successfully!");
    }

    [Fact]
    public async Task OnPostAsync_WhenAddDoctorFails_SetsErrorMessage()
    {
        // Arrange
        SetupAdminSession();
        SetupDepartments();

        _sut.Input = new DoctorRegistrationViewModel
        {
            Name = "Dr. Fail",
            Email = "fail@clinic.com",
            Password = "Doc@123",
            BirthDate = "1985-01-15",
            DeptNo = 1,
            Gender = "M",
            WorkExperience = 5,
            Salary = 70000,
            ChargesPerVisit = 400
        };

        _doctorServiceMock.Setup(s => s.EmailExistsAsync("fail@clinic.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _doctorServiceMock.Setup(s => s.AddDoctorAsync(It.IsAny<DomainDoctor>(), "fail@clinic.com", "Doc@123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.ErrorMessage.Should().Be("Failed to register doctor. Please try again.");
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_SetsErrorMessage()
    {
        // Arrange
        SetupAdminSession();
        SetupDepartments();

        _sut.Input = new DoctorRegistrationViewModel
        {
            Name = "Dr. Error",
            Email = "error@clinic.com",
            Password = "Doc@123",
            BirthDate = "1985-01-15",
            DeptNo = 1,
            Gender = "M",
            WorkExperience = 5,
            Salary = 70000,
            ChargesPerVisit = 400
        };

        _doctorServiceMock.Setup(s => s.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.ErrorMessage.Should().Be("An error occurred during registration.");
    }
}
