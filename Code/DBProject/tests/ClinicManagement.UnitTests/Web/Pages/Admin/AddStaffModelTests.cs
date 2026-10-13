using ClinicManagement.Domain.Entities;
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

namespace ClinicManagement.UnitTests.Web.Pages.Admin;

/// <summary>
/// Unit tests for AddStaffModel page model.
/// </summary>
public class AddStaffModelTests
{
    private readonly Mock<IStaffService> _staffServiceMock;
    private readonly Mock<ILogger<AddStaffModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly AddStaffModel _sut;

    public AddStaffModelTests()
    {
        _staffServiceMock = new Mock<IStaffService>();
        _loggerMock = new Mock<ILogger<AddStaffModel>>();
        _sessionMock = new Mock<ISession>();

        _sut = new AddStaffModel(_staffServiceMock.Object, _loggerMock.Object);

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
        _sut.Input.Should().NotBeNull();
    }

    [Fact]
    public void OnGet_WhenNotAdmin_RedirectsToLogin()
    {
        // Arrange
        SetupNonAdminSession();

        // Act
        var result = _sut.OnGet();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirect = (RedirectToPageResult)result;
        redirect.PageName.Should().Be("/Account/Login");
    }

    [Fact]
    public void OnGet_WhenAdmin_ReturnsPage()
    {
        // Arrange
        SetupAdminSession();

        // Act
        var result = _sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WhenNotAdmin_RedirectsToLogin()
    {
        // Arrange
        SetupNonAdminSession();

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
        _sut.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WithValidInput_AddsStaffAndSetsSuccessMessage()
    {
        // Arrange
        SetupAdminSession();
        _sut.Input = new AddStaffViewModel
        {
            Name = "Bob Nurse",
            BirthDate = "1985-03-10",
            Phone = "5551234567",
            Gender = "M",
            Address = "789 Pine Rd",
            Salary = 40000,
            Qualification = "BSc Nursing",
            Designation = "Nurse"
        };

        _staffServiceMock.Setup(s => s.AddStaffAsync(It.IsAny<OtherStaff>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.SuccessMessage.Should().Be("Staff member added successfully!");
    }

    [Fact]
    public async Task OnPostAsync_WhenAddFails_SetsErrorMessage()
    {
        // Arrange
        SetupAdminSession();
        _sut.Input = new AddStaffViewModel
        {
            Name = "Bob Nurse",
            BirthDate = "1985-03-10",
            Gender = "M",
            Salary = 40000
        };

        _staffServiceMock.Setup(s => s.AddStaffAsync(It.IsAny<OtherStaff>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.ErrorMessage.Should().Be("Failed to add staff member.");
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_SetsErrorMessage()
    {
        // Arrange
        SetupAdminSession();
        _sut.Input = new AddStaffViewModel
        {
            Name = "Bob Nurse",
            BirthDate = "1985-03-10",
            Gender = "M",
            Salary = 40000
        };

        _staffServiceMock.Setup(s => s.AddStaffAsync(It.IsAny<OtherStaff>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.ErrorMessage.Should().Be("An error occurred while adding staff member.");
    }

    [Fact]
    public async Task OnPostAsync_WithEmptyGender_UsesDefaultMale()
    {
        // Arrange
        SetupAdminSession();
        _sut.Input = new AddStaffViewModel
        {
            Name = "Test Staff",
            BirthDate = "1990-01-01",
            Gender = "",
            Salary = 30000
        };

        OtherStaff? capturedStaff = null;
        _staffServiceMock.Setup(s => s.AddStaffAsync(It.IsAny<OtherStaff>(), It.IsAny<CancellationToken>()))
            .Callback<OtherStaff, CancellationToken>((staff, _) => capturedStaff = staff)
            .ReturnsAsync(true);

        // Act
        await _sut.OnPostAsync();

        // Assert
        capturedStaff.Should().NotBeNull();
        capturedStaff!.Gender.Should().Be('M');
    }
}
