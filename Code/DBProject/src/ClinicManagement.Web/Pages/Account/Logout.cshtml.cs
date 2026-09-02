using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Account;

/// <summary>
/// Page model for logout.
/// </summary>
public class LogoutModel : PageModel
{
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(ILogger<LogoutModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        HttpContext.Session.Clear();
        _logger.LogInformation("User {UserId} logged out", userId);
        return RedirectToPage("/Account/Login");
    }

    public IActionResult OnPost()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        HttpContext.Session.Clear();
        _logger.LogInformation("User {UserId} logged out", userId);
        return RedirectToPage("/Account/Login");
    }
}
