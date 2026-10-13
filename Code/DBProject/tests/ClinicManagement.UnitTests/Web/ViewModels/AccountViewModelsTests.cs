using ClinicManagement.Web.ViewModels;
using FluentAssertions;
using Xunit;

namespace ClinicManagement.UnitTests.Web.ViewModels;

/// <summary>
/// Unit tests for AccountViewModels (LoginViewModel and SignUpViewModel).
/// </summary>
public class AccountViewModelsTests
{
    // ─── LoginViewModel ───────────────────────────────────────────────────────

    [Fact]
    public void LoginViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new LoginViewModel();

        // Assert
        vm.Email.Should().Be(string.Empty);
        vm.Password.Should().Be(string.Empty);
        vm.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void LoginViewModel_SetEmail_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new LoginViewModel();

        // Act
        vm.Email = "test@example.com";

        // Assert
        vm.Email.Should().Be("test@example.com");
    }

    [Fact]
    public void LoginViewModel_SetPassword_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new LoginViewModel();

        // Act
        vm.Password = "SecurePass123";

        // Assert
        vm.Password.Should().Be("SecurePass123");
    }

    [Fact]
    public void LoginViewModel_SetErrorMessage_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new LoginViewModel();

        // Act
        vm.ErrorMessage = "Invalid credentials";

        // Assert
        vm.ErrorMessage.Should().Be("Invalid credentials");
    }

    [Fact]
    public void LoginViewModel_SetAllProperties_AllValuesCorrect()
    {
        // Arrange & Act
        var vm = new LoginViewModel
        {
            Email = "admin@clinic.com",
            Password = "Admin@123",
            ErrorMessage = "Some error"
        };

        // Assert
        vm.Email.Should().Be("admin@clinic.com");
        vm.Password.Should().Be("Admin@123");
        vm.ErrorMessage.Should().Be("Some error");
    }

    // ─── SignUpViewModel ──────────────────────────────────────────────────────

    [Fact]
    public void SignUpViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new SignUpViewModel();

        // Assert
        vm.Name.Should().Be(string.Empty);
        vm.BirthDate.Should().Be(string.Empty);
        vm.Email.Should().Be(string.Empty);
        vm.Password.Should().Be(string.Empty);
        vm.ConfirmPassword.Should().Be(string.Empty);
        vm.PhoneNo.Should().Be(string.Empty);
        vm.Gender.Should().Be(string.Empty);
        vm.Address.Should().Be(string.Empty);
        vm.ErrorMessage.Should().BeNull();
        vm.SuccessMessage.Should().BeNull();
    }

    [Fact]
    public void SignUpViewModel_SetAllProperties_AllValuesCorrect()
    {
        // Arrange & Act
        var vm = new SignUpViewModel
        {
            Name = "John Doe",
            BirthDate = "1990-01-15",
            Email = "john@example.com",
            Password = "Pass@123",
            ConfirmPassword = "Pass@123",
            PhoneNo = "1234567890",
            Gender = "M",
            Address = "123 Main St",
            ErrorMessage = null,
            SuccessMessage = "Registered!"
        };

        // Assert
        vm.Name.Should().Be("John Doe");
        vm.BirthDate.Should().Be("1990-01-15");
        vm.Email.Should().Be("john@example.com");
        vm.Password.Should().Be("Pass@123");
        vm.ConfirmPassword.Should().Be("Pass@123");
        vm.PhoneNo.Should().Be("1234567890");
        vm.Gender.Should().Be("M");
        vm.Address.Should().Be("123 Main St");
        vm.ErrorMessage.Should().BeNull();
        vm.SuccessMessage.Should().Be("Registered!");
    }

    [Fact]
    public void SignUpViewModel_SetErrorMessage_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new SignUpViewModel();

        // Act
        vm.ErrorMessage = "Email already exists";

        // Assert
        vm.ErrorMessage.Should().Be("Email already exists");
    }

    [Fact]
    public void SignUpViewModel_SetSuccessMessage_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new SignUpViewModel();

        // Act
        vm.SuccessMessage = "Registration successful!";

        // Assert
        vm.SuccessMessage.Should().Be("Registration successful!");
    }

    [Fact]
    public void SignUpViewModel_PasswordAndConfirmPassword_CanBeCompared()
    {
        // Arrange
        var vm = new SignUpViewModel
        {
            Password = "MyPass@123",
            ConfirmPassword = "MyPass@123"
        };

        // Assert
        vm.Password.Should().Be(vm.ConfirmPassword);
    }

    [Fact]
    public void SignUpViewModel_MismatchedPasswords_AreNotEqual()
    {
        // Arrange
        var vm = new SignUpViewModel
        {
            Password = "MyPass@123",
            ConfirmPassword = "DifferentPass"
        };

        // Assert
        vm.Password.Should().NotBe(vm.ConfirmPassword);
    }
}
