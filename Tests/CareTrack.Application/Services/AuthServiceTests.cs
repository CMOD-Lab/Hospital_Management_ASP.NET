using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using CareTrack.Application.Services;
using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Repositories;

namespace CareTrack.Application.Services.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _mockAuthRepo;
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _mockAuthRepo = new Mock<IAuthRepository>();
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockLogger = new Mock<ILogger<AuthService>>();

            _service = new AuthService(
                _mockAuthRepo.Object,
                _mockPatientRepo.Object,
                _mockLogger.Object);
        }

        // ValidateLoginAsync Tests
        [Fact]
        public async Task ValidateLoginAsync_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var login = new LoginTable { LoginId = 1, Email = "user@test.com", Type = 1 };
            _mockAuthRepo.Setup(r => r.ValidateLoginAsync("user@test.com", "password", It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            // Act
            var (success, userId, userType, message) = await _service.ValidateLoginAsync("user@test.com", "password");

            // Assert
            Assert.True(success);
            Assert.Equal(1, userId);
            Assert.Equal(1, userType);
            Assert.Contains("successful", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ValidateLoginAsync_WithInvalidCredentials_ReturnsFailure()
        {
            // Arrange
            _mockAuthRepo.Setup(r => r.ValidateLoginAsync("wrong@test.com", "wrongpass", It.IsAny<CancellationToken>()))
                .ReturnsAsync((LoginTable?)null);

            // Act
            var (success, userId, userType, message) = await _service.ValidateLoginAsync("wrong@test.com", "wrongpass");

            // Assert
            Assert.False(success);
            Assert.Equal(0, userId);
            Assert.Equal(0, userType);
            Assert.Contains("Invalid", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ValidateLoginAsync_WithEmptyEmail_ReturnsFailure()
        {
            // Arrange & Act
            var (success, userId, userType, message) = await _service.ValidateLoginAsync(string.Empty, "password");

            // Assert
            Assert.False(success);
            Assert.Equal(0, userId);
            Assert.Contains("required", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ValidateLoginAsync_WithEmptyPassword_ReturnsFailure()
        {
            // Arrange & Act
            var (success, userId, userType, message) = await _service.ValidateLoginAsync("user@test.com", string.Empty);

            // Assert
            Assert.False(success);
            Assert.Equal(0, userId);
            Assert.Contains("required", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ValidateLoginAsync_WithNullEmail_ReturnsFailure()
        {
            // Arrange & Act
            var (success, userId, userType, message) = await _service.ValidateLoginAsync(null!, "password");

            // Assert
            Assert.False(success);
            Assert.Equal(0, userId);
        }

        [Fact]
        public async Task ValidateLoginAsync_WithWhitespaceEmail_ReturnsFailure()
        {
            // Arrange & Act
            var (success, userId, userType, message) = await _service.ValidateLoginAsync("   ", "password");

            // Assert
            Assert.False(success);
            Assert.Equal(0, userId);
        }

        [Fact]
        public async Task ValidateLoginAsync_WhenRepositoryThrows_ReturnsFailure()
        {
            // Arrange
            _mockAuthRepo.Setup(r => r.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var (success, userId, userType, message) = await _service.ValidateLoginAsync("user@test.com", "password");

            // Assert
            Assert.False(success);
            Assert.Equal(0, userId);
            Assert.NotEmpty(message);
        }

        [Fact]
        public async Task ValidateLoginAsync_DoctorLogin_ReturnsCorrectUserType()
        {
            // Arrange
            var login = new LoginTable { LoginId = 5, Email = "doctor@test.com", Type = 2 };
            _mockAuthRepo.Setup(r => r.ValidateLoginAsync("doctor@test.com", "docpass", It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            // Act
            var (success, userId, userType, message) = await _service.ValidateLoginAsync("doctor@test.com", "docpass");

            // Assert
            Assert.True(success);
            Assert.Equal(5, userId);
            Assert.Equal(2, userType);
        }

        [Fact]
        public async Task ValidateLoginAsync_AdminLogin_ReturnsCorrectUserType()
        {
            // Arrange
            var login = new LoginTable { LoginId = 99, Email = "admin@test.com", Type = 3 };
            _mockAuthRepo.Setup(r => r.ValidateLoginAsync("admin@test.com", "adminpass", It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            // Act
            var (success, userId, userType, message) = await _service.ValidateLoginAsync("admin@test.com", "adminpass");

            // Assert
            Assert.True(success);
            Assert.Equal(99, userId);
            Assert.Equal(3, userType);
        }

        // RegisterPatientAsync Tests
        [Fact]
        public async Task RegisterPatientAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var login = new LoginTable { LoginId = 10, Email = "newpatient@test.com", Type = 1 };
            _mockAuthRepo.Setup(r => r.EmailExistsAsync("newpatient@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mockAuthRepo.Setup(r => r.CreateLoginAsync(It.IsAny<LoginTable>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);
            _mockPatientRepo.Setup(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Patient { PatientId = 10, Name = "John Doe" });

            // Act
            var (success, patientId, message) = await _service.RegisterPatientAsync(
                "John Doe", "1990-01-15", "newpatient@test.com", "password",
                "555-1234", "M", "123 Main St");

            // Assert
            Assert.True(success);
            Assert.Equal(10, patientId);
            Assert.Contains("successful", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task RegisterPatientAsync_WithExistingEmail_ReturnsFailure()
        {
            // Arrange
            _mockAuthRepo.Setup(r => r.EmailExistsAsync("existing@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var (success, patientId, message) = await _service.RegisterPatientAsync(
                "John Doe", "1990-01-15", "existing@test.com", "password",
                "555-1234", "M", "123 Main St");

            // Assert
            Assert.False(success);
            Assert.Equal(0, patientId);
            Assert.Contains("already exists", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task RegisterPatientAsync_WithInvalidBirthDate_ReturnsFailure()
        {
            // Arrange
            _mockAuthRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var (success, patientId, message) = await _service.RegisterPatientAsync(
                "John Doe", "not-a-date", "new@test.com", "password",
                "555-1234", "M", "123 Main St");

            // Assert
            Assert.False(success);
            Assert.Equal(0, patientId);
            Assert.Contains("Invalid birth date", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task RegisterPatientAsync_WhenRepositoryThrows_ReturnsFailure()
        {
            // Arrange
            _mockAuthRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var (success, patientId, message) = await _service.RegisterPatientAsync(
                "John Doe", "1990-01-15", "error@test.com", "password",
                "555-1234", "M", "123 Main St");

            // Assert
            Assert.False(success);
            Assert.Equal(0, patientId);
            Assert.NotEmpty(message);
        }

        [Fact]
        public async Task RegisterPatientAsync_CreatesPatientWithCorrectData()
        {
            // Arrange
            Patient? capturedPatient = null;
            var login = new LoginTable { LoginId = 15, Email = "test@test.com", Type = 1 };

            _mockAuthRepo.Setup(r => r.EmailExistsAsync("test@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mockAuthRepo.Setup(r => r.CreateLoginAsync(It.IsAny<LoginTable>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);
            _mockPatientRepo.Setup(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient p, CancellationToken ct) =>
                {
                    capturedPatient = p;
                    return p;
                });

            // Act
            await _service.RegisterPatientAsync(
                "Jane Smith", "1995-06-20", "test@test.com", "password",
                "555-9999", "F", "456 Oak Ave");

            // Assert
            Assert.NotNull(capturedPatient);
            Assert.Equal("Jane Smith", capturedPatient.Name);
            Assert.Equal("F", capturedPatient.Gender);
            Assert.Equal("555-9999", capturedPatient.Phone);
            Assert.Equal("456 Oak Ave", capturedPatient.Address);
        }

        [Fact]
        public async Task RegisterPatientAsync_WithValidDateFormats_Succeeds()
        {
            // Arrange
            var login = new LoginTable { LoginId = 20, Email = "date@test.com", Type = 1 };
            _mockAuthRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mockAuthRepo.Setup(r => r.CreateLoginAsync(It.IsAny<LoginTable>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);
            _mockPatientRepo.Setup(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Patient { PatientId = 20 });

            // Act
            var (success, _, _) = await _service.RegisterPatientAsync(
                "Test User", "2000-12-31", "date@test.com", "pass",
                "555-0000", "M", "Test Address");

            // Assert
            Assert.True(success);
        }
    }
}
