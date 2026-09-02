using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Account;

/// <summary>
/// Page model for the login page.
/// </summary>
public class LoginModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IAuthService authService, ILogger<LoginModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [BindProperty]
    public LoginViewModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        // If already logged in, redirect appropriately
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId.HasValue)
        {
            var userType = HttpContext.Session.GetInt32("UserType");
            return RedirectBasedOnUserType(userType ?? 0);
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var (success, userId, userType) = await _authService.ValidateLoginAsync(
                Input.Email, Input.Password);

            if (!success)
            {
                ErrorMessage = "Invalid email or password. Please try again.";
                return Page();
            }

            // Store user info in session
            HttpContext.Session.SetInt32("UserId", userId);
            HttpContext.Session.SetInt32("UserType", (int)userType);
            HttpContext.Session.SetString("UserEmail", Input.Email);

            _logger.LogInformation("User {UserId} logged in successfully", userId);
            return RedirectBasedOnUserType((int)userType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", Input.Email);
            ErrorMessage = "An error occurred during login. Please try again.";
            return Page();
        }
    }

    private IActionResult RedirectBasedOnUserType(int userType)
    {
        return userType switch
        {
            (int)UserType.Patient => RedirectToPage("/Patient/PatientHome"),
            (int)UserType.Doctor => RedirectToPage("/Doctor/DoctorHome"),
            (int)UserType.Admin => RedirectToPage("/Admin/AdminHome"),
            _ => RedirectToPage("/Account/Login")
        };
    }
}
