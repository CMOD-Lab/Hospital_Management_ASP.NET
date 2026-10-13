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
/// Unit tests for SignUpModel page model.
/// </summary>
public class SignUpModelTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<ILogger<SignUpModel>> _loggerMock;
    private readonly SignUpModel _sut;

    public SignUpModelTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _loggerMock = new Mock<ILogger<SignUpModel>>();

        _sut = new SignUpModel(_authServiceMock.Object, _loggerMock.Object);

        var pageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext()
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
    public void OnGet_ReturnsPage()
    {
        // Act
        var result = _sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidModelState_ReturnsPage()
    {
        // Arrange
        _sut.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WithSuccessfulRegistration_RedirectsToLogin()
    {
        // Arrange
        _sut.Input = new SignUpViewModel
        {
            Name = "John Doe",
            BirthDate = "1990-01-01",
            Email = "john@test.com",
            Password = "Pass@123",
            ConfirmPassword = "Pass@123",
            PhoneNo = "1234567890",
            Gender = "M",
            Address = "123 Main St"
        };

        _authServiceMock
            .Setup(s => s.RegisterPatientAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, 42, "Success"));

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirect = (RedirectToPageResult)result;
        redirect.PageName.Should().Be("/Account/Login");
    }

    [Fact]
    public async Task OnPostAsync_WithFailedRegistration_ReturnsPageWithError()
    {
        // Arrange
        _sut.Input = new SignUpViewModel
        {
            Name = "John Doe",
            BirthDate = "1990-01-01",
            Email = "existing@test.com",
            Password = "Pass@123",
            ConfirmPassword = "Pass@123",
            PhoneNo = "1234567890",
            Gender = "M",
            Address = "123 Main St"
        };

        _authServiceMock
            .Setup(s => s.RegisterPatientAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, 0, "Email already exists"));

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.ErrorMessage.Should().Be("Email already exists");
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_ReturnsPageWithError()
    {
        // Arrange
        _sut.Input = new SignUpViewModel
        {
            Name = "John Doe",
            BirthDate = "1990-01-01",
            Email = "john@test.com",
            Password = "Pass@123",
            ConfirmPassword = "Pass@123",
            PhoneNo = "1234567890",
            Gender = "M",
            Address = "123 Main St"
        };

        _authServiceMock
            .Setup(s => s.RegisterPatientAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.ErrorMessage.Should().Be("An error occurred during registration. Please try again.");
    }

    [Fact]
    public async Task OnPostAsync_SuccessfulRegistration_SetsSuccessMessage()
    {
        // Arrange
        _sut.Input = new SignUpViewModel
        {
            Name = "Jane",
            BirthDate = "1995-05-05",
            Email = "jane@test.com",
            Password = "Pass@123",
            ConfirmPassword = "Pass@123",
            PhoneNo = "9876543210",
            Gender = "F",
            Address = "456 Oak Ave"
        };

        _authServiceMock
            .Setup(s => s.RegisterPatientAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, 99, "Success"));

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
    }
}
