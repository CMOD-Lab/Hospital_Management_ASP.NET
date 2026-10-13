using ClinicManagement.Web.Pages.Account;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Account;

/// <summary>
/// Unit tests for LogoutModel page model.
/// </summary>
public class LogoutModelTests
{
    private readonly Mock<ILogger<LogoutModel>> _loggerMock;
    private readonly Mock<ISession> _sessionMock;
    private readonly LogoutModel _sut;

    public LogoutModelTests()
    {
        _loggerMock = new Mock<ILogger<LogoutModel>>();
        _sessionMock = new Mock<ISession>();

        var httpContext = new DefaultHttpContext();
        // We need a real session mock since DefaultHttpContext doesn't have a real session
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_sessionMock.Object);

        _sut = new LogoutModel(_loggerMock.Object);
        _sut.PageContext = new PageContext
        {
            HttpContext = mockHttpContext.Object
        };
    }

    [Fact]
    public void Constructor_WithValidLogger_CreatesInstance()
    {
        // Assert
        _sut.Should().NotBeNull();
    }

    [Fact]
    public void OnGet_ClearsSessionAndRedirectsToLogin()
    {
        // Arrange
        byte[]? userIdBytes = null;
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(false);
        _sessionMock.Setup(s => s.Clear());

        // Act
        var result = _sut.OnGet();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirect = (RedirectToPageResult)result;
        redirect.PageName.Should().Be("/Account/Login");
        _sessionMock.Verify(s => s.Clear(), Times.Once);
    }

    [Fact]
    public void OnPost_ClearsSessionAndRedirectsToLogin()
    {
        // Arrange
        byte[]? userIdBytes = null;
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(false);
        _sessionMock.Setup(s => s.Clear());

        // Act
        var result = _sut.OnPost();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirect = (RedirectToPageResult)result;
        redirect.PageName.Should().Be("/Account/Login");
        _sessionMock.Verify(s => s.Clear(), Times.Once);
    }

    [Fact]
    public void OnGet_WhenUserIdInSession_ClearsSessionAndRedirects()
    {
        // Arrange
        var userIdBytes = BitConverter.GetBytes(5);
        if (BitConverter.IsLittleEndian) Array.Reverse(userIdBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(true);
        _sessionMock.Setup(s => s.Clear());

        // Act
        var result = _sut.OnGet();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        _sessionMock.Verify(s => s.Clear(), Times.Once);
    }

    [Fact]
    public void OnPost_WhenUserIdInSession_ClearsSessionAndRedirects()
    {
        // Arrange
        var userIdBytes = BitConverter.GetBytes(10);
        if (BitConverter.IsLittleEndian) Array.Reverse(userIdBytes);
        _sessionMock.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(true);
        _sessionMock.Setup(s => s.Clear());

        // Act
        var result = _sut.OnPost();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        _sessionMock.Verify(s => s.Clear(), Times.Once);
    }
}
