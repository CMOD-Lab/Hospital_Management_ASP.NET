using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages;

/// <summary>Handles login and patient sign-up operations.</summary>
public class SignUpModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<SignUpModel> _logger;

    public SignUpModel(IAuthService authService, ILogger<SignUpModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    // Login form fields
    public string LoginEmail { get; set; } = string.Empty;

    // Sign-up form fields
    public string SignUpName { get; set; } = string.Empty;
    public string SignUpBirthDate { get; set; } = string.Empty;
    public string SignUpEmail { get; set; } = string.Empty;
    public string SignUpPhone { get; set; } = string.Empty;
    public string SignUpAddress { get; set; } = string.Empty;

    public void OnGet()
    {
        // Clear session on page load (logout)
    }

    public async Task<IActionResult> OnGetLogout()
    {
        HttpContext.Session.Clear();
        _logger.LogInformation("User logged out");
        return RedirectToPage("/SignUp");
    }

    /// <summary>Handles login form submission.</summary>
    public async Task<IActionResult> OnPostLoginAsync(
        string loginEmail,
        string loginPassword,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(loginEmail) || string.IsNullOrWhiteSpace(loginPassword))
        {
            ErrorMessage = "Please enter both email and password.";
            LoginEmail = loginEmail ?? string.Empty;
            return Page();
        }

        var result = await _authService.ValidateLoginAsync(loginEmail, loginPassword, cancellationToken);

        if (!result.Success)
        {
            ErrorMessage = result.ErrorMessage ?? "Login failed. Please try again.";
            LoginEmail = loginEmail;
            return Page();
        }

        // Store user info in session
        HttpContext.Session.SetInt32("UserId", result.UserId);
        HttpContext.Session.SetInt32("UserType", result.UserType);

        _logger.LogInformation("User {UserId} logged in as type {UserType}", result.UserId, result.UserType);

        // Redirect based on user type
        return result.UserType switch
        {
            1 => RedirectToPage("/Patient/PatientHome"),
            2 => RedirectToPage("/Doctor/DoctorHome"),
            3 => RedirectToPage("/Admin/AdminHome"),
            _ => RedirectToPage("/SignUp")
        };
    }

    /// <summary>Handles patient sign-up form submission.</summary>
    public async Task<IActionResult> OnPostSignUpAsync(
        string sName,
        string sBirthDate,
        string sEmail,
        string sPassword,
        string sPhone,
        string sAddress,
        string Gender,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sName) || string.IsNullOrWhiteSpace(sEmail) || string.IsNullOrWhiteSpace(sPassword))
        {
            ErrorMessage = "Please fill in all required fields.";
            SignUpName = sName ?? string.Empty;
            SignUpEmail = sEmail ?? string.Empty;
            SignUpPhone = sPhone ?? string.Empty;
            SignUpAddress = sAddress ?? string.Empty;
            return Page();
        }

        var dto = new PatientSignUpDto
        {
            Name = sName,
            BirthDate = sBirthDate,
            Email = sEmail,
            Password = sPassword,
            PhoneNo = sPhone ?? string.Empty,
            Gender = Gender ?? "M",
            Address = sAddress ?? string.Empty
        };

        var result = await _authService.SignUpPatientAsync(dto, cancellationToken);

        if (!result.Success)
        {
            ErrorMessage = result.ErrorMessage ?? "Sign-up failed. Please try again.";
            SignUpName = sName;
            SignUpEmail = sEmail;
            SignUpPhone = sPhone ?? string.Empty;
            SignUpAddress = sAddress ?? string.Empty;
            return Page();
        }

        // Store user info in session
        HttpContext.Session.SetInt32("UserId", result.PatientId);
        HttpContext.Session.SetInt32("UserType", 1);

        _logger.LogInformation("New patient registered with ID: {PatientId}", result.PatientId);
        return RedirectToPage("/Patient/PatientHome");
    }
}
