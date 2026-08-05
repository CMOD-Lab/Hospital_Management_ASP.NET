using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Services;

/// <summary>Unit tests for AuthService.</summary>
public class AuthServiceTests
{
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _loginRepoMock = new Mock<ILoginRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _authService = new AuthService(_loginRepoMock.Object, _patientRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var login = new LoginTable { LoginId = 1, Email = "test@test.com", Password = "pass123", Type = 1 };
        _loginRepoMock.Setup(r => r.ValidateLoginAsync("test@test.com", "pass123", default))
                      .ReturnsAsync(login);

        // Act
        var result = await _authService.ValidateLoginAsync("test@test.com", "pass123");

        // Assert
        result.Success.Should().BeTrue();
        result.UserId.Should().Be(1);
        result.UserType.Should().Be(1);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithInvalidCredentials_ReturnsFailure()
    {
        // Arrange
        _loginRepoMock.Setup(r => r.ValidateLoginAsync("bad@test.com", "wrong", default))
                      .ReturnsAsync((LoginTable?)null);

        // Act
        var result = await _authService.ValidateLoginAsync("bad@test.com", "wrong");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SignUpPatientAsync_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        _loginRepoMock.Setup(r => r.EmailExistsAsync("existing@test.com", default))
                      .ReturnsAsync(true);

        var dto = new PatientSignUpDto
        {
            Name = "Test Patient",
            Email = "existing@test.com",
            Password = "pass123",
            BirthDate = "1990-01-01",
            Gender = "M"
        };

        // Act
        var result = await _authService.SignUpPatientAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("already exists");
    }

    [Fact]
    public async Task SignUpPatientAsync_WithNewEmail_ReturnsSuccess()
    {
        // Arrange
        _loginRepoMock.Setup(r => r.EmailExistsAsync("new@test.com", default))
                      .ReturnsAsync(false);
        _loginRepoMock.Setup(r => r.AddAsync(It.IsAny<LoginTable>(), default))
                      .ReturnsAsync(42);
        _patientRepoMock.Setup(r => r.AddAsync(It.IsAny<Patient>(), default))
                        .Returns(Task.CompletedTask);

        var dto = new PatientSignUpDto
        {
            Name = "New Patient",
            Email = "new@test.com",
            Password = "pass123",
            BirthDate = "1990-01-01",
            Gender = "M",
            PhoneNo = "1234567890",
            Address = "123 Main St"
        };

        // Act
        var result = await _authService.SignUpPatientAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        result.PatientId.Should().Be(42);
    }
}
