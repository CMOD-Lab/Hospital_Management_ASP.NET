using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.Pages.Account;
using ClinicManagement.Web.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Account;

/// <summary>
/// Unit tests for LoginModel page model.
/// </summary>
public class LoginModelTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<ILogger<LoginModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly Mock<HttpContext> _httpContextMock;
    private readonly LoginModel _sut;

    public LoginModelTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _loggerMock = new Mock<ILogger<LoginModel>>();
        _sessionMock = new Mock<ISession>();
        _httpContextMock = new Mock<HttpContext>();

        _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);

        _sut = new LoginModel(_authServiceMock.Object, _loggerMock.Object);

        // Inject HttpContext via PageContext
        var pageContext = new PageContext
        {
            HttpContext = _httpContextMock.Object
        };
        _sut.PageContext = pageContext;
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Assert
        _sut.Should().NotBeNull();
        _sut.Input.Should().NotBeNull();
    }

    [Fact]
    public void OnGet_WhenNoSession_ReturnsPage()
    {
        // Arrange
        byte[]? outBytes = null;
        _sessionMock.Setup(s => s.TryGetValue("UserId", out outBytes)).Returns(false);

        // Act
        var result = _sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public void OnGet_WhenUserAlreadyLoggedInAsPatient_RedirectsToPatientHome()
    {
        // Arrange
        var userIdBytes = BitConverter.GetBytes(1);
        var userTypeBytes = BitConverter.GetBytes((int)UserType.Patient);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(userIdBytes);
            Array.Reverse(userTypeBytes);
        }

        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(true);
        _sessionMock.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);

        // Act
        var result = _sut.OnGet();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidModelState_ReturnsPage()
    {
        // Arrange
        _sut.ModelState.AddModelError("Email", "Required");

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidCredentials_ReturnsPageWithError()
    {
        // Arrange
        _sut.Input = new LoginViewModel { Email = "wrong@test.com", Password = "wrong" };
        _authServiceMock
            .Setup(s => s.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, 0, UserType.Patient));

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.ErrorMessage.Should().Be("Invalid email or password. Please try again.");
    }

    [Fact]
    public async Task OnPostAsync_WithValidPatientCredentials_RedirectsToPatientHome()
    {
        // Arrange
        _sut.Input = new LoginViewModel { Email = "patient@test.com", Password = "pass" };
        _authServiceMock
            .Setup(s => s.ValidateLoginAsync("patient@test.com", "pass", It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, 10, UserType.Patient));

        _sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()));

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirect = (RedirectToPageResult)result;
        redirect.PageName.Should().Be("/Patient/PatientHome");
    }

    [Fact]
    public async Task OnPostAsync_WithValidDoctorCredentials_RedirectsToDoctorHome()
    {
        // Arrange
        _sut.Input = new LoginViewModel { Email = "doctor@test.com", Password = "pass" };
        _authServiceMock
            .Setup(s => s.ValidateLoginAsync("doctor@test.com", "pass", It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, 5, UserType.Doctor));

        _sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()));

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirect = (RedirectToPageResult)result;
        redirect.PageName.Should().Be("/Doctor/DoctorHome");
    }

    [Fact]
    public async Task OnPostAsync_WithValidAdminCredentials_RedirectsToAdminHome()
    {
        // Arrange
        _sut.Input = new LoginViewModel { Email = "admin@test.com", Password = "pass" };
        _authServiceMock
            .Setup(s => s.ValidateLoginAsync("admin@test.com", "pass", It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, 1, UserType.Admin));

        _sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()));

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirect = (RedirectToPageResult)result;
        redirect.PageName.Should().Be("/Admin/AdminHome");
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_ReturnsPageWithError()
    {
        // Arrange
        _sut.Input = new LoginViewModel { Email = "test@test.com", Password = "pass" };
        _authServiceMock
            .Setup(s => s.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.ErrorMessage.Should().Be("An error occurred during login. Please try again.");
    }
}
