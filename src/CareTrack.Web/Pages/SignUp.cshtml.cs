using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareTrack.Web.Pages;

/// <summary>
/// Page model for the login and sign-up page.
/// </summary>
public class SignUpModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<SignUpModel> _logger;

    public string LoginMessage { get; set; } = string.Empty;
    public string SignUpMessage { get; set; } = string.Empty;
    public bool SignUpSuccess { get; set; } = false;

    public SignUpModel(IAuthService authService, ILogger<SignUpModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public void OnGet()
    {
        // Clear session on page load (logout behavior)
        HttpContext.Session.Clear();
    }

    /// <summary>
    /// Handles login form submission.
    /// </summary>
    public async Task<IActionResult> OnPostLoginAsync(
        string loginEmail, string loginPassword, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(loginEmail) || string.IsNullOrWhiteSpace(loginPassword))
        {
            LoginMessage = "Email and password are required.";
            return Page();
        }

        var (success, userId, userType, message) = await _authService.ValidateLoginAsync(
            loginEmail, loginPassword, cancellationToken);

        if (!success)
        {
            LoginMessage = message;
            return Page();
        }

        // Store user info in session
        HttpContext.Session.SetInt32("UserId", userId);
        HttpContext.Session.SetInt32("UserType", userType);

        _logger.LogInformation("User {UserId} of type {UserType} logged in", userId, userType);

        // Redirect based on user type
        return userType switch
        {
            1 => RedirectToPage("/Patient/PatientHome"),
            2 => RedirectToPage("/Doctor/DoctorHome"),
            3 => RedirectToPage("/Admin/AdminHome"),
            _ => Page()
        };
    }

    /// <summary>
    /// Handles patient sign-up form submission.
    /// </summary>
    public async Task<IActionResult> OnPostSignUpAsync(
        string sName, string sBirthDate, string sEmail, string sPassword,
        string sPhone, string sGender, string sAddress, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sName) || string.IsNullOrWhiteSpace(sEmail) ||
            string.IsNullOrWhiteSpace(sPassword))
        {
            SignUpMessage = "All required fields must be filled.";
            return Page();
        }

        var (success, patientId, message) = await _authService.RegisterPatientAsync(
            sName, sBirthDate, sEmail, sPassword, sPhone, sGender, sAddress, cancellationToken);

        if (!success)
        {
            SignUpMessage = message;
            return Page();
        }

        // Auto-login after registration
        HttpContext.Session.SetInt32("UserId", patientId);
        HttpContext.Session.SetInt32("UserType", 1);

        _logger.LogInformation("New patient registered and logged in with ID: {PatientId}", patientId);

        return RedirectToPage("/Patient/PatientHome");
    }
}
