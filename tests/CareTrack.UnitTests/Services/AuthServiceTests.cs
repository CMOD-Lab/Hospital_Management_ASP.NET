using CareTrack.Application.Services;
using Xunit;
using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CareTrack.UnitTests.Services;

/// <summary>
/// Unit tests for the AuthService.
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _authRepositoryMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authRepositoryMock = new Mock<IAuthRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _authService = new AuthService(
            _authRepositoryMock.Object,
            _patientRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var login = new LoginTable { LoginId = 1, Email = "test@test.com", Password = "pass123", Type = 1 };
        _authRepositoryMock.Setup(r => r.ValidateLoginAsync("test@test.com", "pass123", default))
            .ReturnsAsync(login);

        // Act
        var (success, userId, userType, message) = await _authService.ValidateLoginAsync("test@test.com", "pass123");

        // Assert
        success.Should().BeTrue();
        userId.Should().Be(1);
        userType.Should().Be(1);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithInvalidCredentials_ReturnsFailure()
    {
        // Arrange
        _authRepositoryMock.Setup(r => r.ValidateLoginAsync("wrong@test.com", "wrongpass", default))
            .ReturnsAsync((LoginTable?)null);

        // Act
        var (success, userId, userType, message) = await _authService.ValidateLoginAsync("wrong@test.com", "wrongpass");

        // Assert
        success.Should().BeFalse();
        userId.Should().Be(0);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithEmptyEmail_ReturnsFailure()
    {
        // Act
        var (success, userId, userType, message) = await _authService.ValidateLoginAsync("", "password");

        // Assert
        success.Should().BeFalse();
        message.Should().NotBeEmpty();
    }

    [Fact]
    public async Task RegisterPatientAsync_WithNewEmail_ReturnsSuccess()
    {
        // Arrange
        _authRepositoryMock.Setup(r => r.EmailExistsAsync("new@test.com", default))
            .ReturnsAsync(false);

        var createdLogin = new LoginTable { LoginId = 5, Email = "new@test.com", Type = 1 };
        _authRepositoryMock.Setup(r => r.CreateLoginAsync(It.IsAny<LoginTable>(), default))
            .ReturnsAsync(createdLogin);

        var createdPatient = new Patient { PatientId = 5, Name = "Test Patient" };
        _patientRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Patient>(), default))
            .ReturnsAsync(createdPatient);

        // Act
        var (success, patientId, message) = await _authService.RegisterPatientAsync(
            "Test Patient", "1990-01-01", "new@test.com", "password",
            "1234567890", "M", "Test Address");

        // Assert
        success.Should().BeTrue();
        patientId.Should().Be(5);
    }

    [Fact]
    public async Task RegisterPatientAsync_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        _authRepositoryMock.Setup(r => r.EmailExistsAsync("existing@test.com", default))
            .ReturnsAsync(true);

        // Act
        var (success, patientId, message) = await _authService.RegisterPatientAsync(
            "Test Patient", "1990-01-01", "existing@test.com", "password",
            "1234567890", "M", "Test Address");

        // Assert
        success.Should().BeFalse();
        message.Should().Contain("already exists");
    }
}
