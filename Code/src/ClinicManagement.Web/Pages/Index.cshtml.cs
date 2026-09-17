using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages;

/// <summary>Login and sign-up page model (migrated from SignUp.aspx.cs).</summary>
public class IndexModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<IndexModel> _logger;

    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;

    public IndexModel(IAuthService authService, ILogger<IndexModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public void OnGet()
    {
        // Clear session on landing page
        HttpContext.Session.Clear();
    }

    /// <summary>Handles login form submission.</summary>
    public async Task<IActionResult> OnPostLoginAsync(string loginEmail, string loginPassword)
    {
        if (string.IsNullOrWhiteSpace(loginEmail) || string.IsNullOrWhiteSpace(loginPassword))
        {
            ErrorMessage = "Email and password are required.";
            return Page();
        }

        try
        {
            var (status, userType, userId) = await _authService.ValidateLoginAsync(loginEmail, loginPassword);

            if (status == 0)
            {
                HttpContext.Session.SetInt32("UserId", userId);
                HttpContext.Session.SetInt32("UserType", (int)userType);

                return userType switch
                {
                    UserType.Patient => RedirectToPage("/Patient/PatientHome"),
                    UserType.Doctor => RedirectToPage("/Doctor/DoctorHome"),
                    UserType.Admin => RedirectToPage("/Admin/AdminHome"),
                    _ => RedirectToPage("/Index")
                };
            }
            else if (status == 1)
            {
                ErrorMessage = "Email not found. Please try again.";
            }
            else if (status == 2)
            {
                ErrorMessage = "Incorrect password. Please try again.";
            }
            else
            {
                ErrorMessage = "An error occurred. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", loginEmail);
            ErrorMessage = "An unexpected error occurred. Please try again.";
        }

        return Page();
    }

    /// <summary>Handles patient sign-up form submission.</summary>
    public async Task<IActionResult> OnPostSignUpAsync(
        string sName, string sBirthDate, string sEmail, string sPassword,
        string Phone, string Gender, string Address)
    {
        if (string.IsNullOrWhiteSpace(sName) || string.IsNullOrWhiteSpace(sEmail) ||
            string.IsNullOrWhiteSpace(sPassword))
        {
            ErrorMessage = "Name, email, and password are required.";
            return Page();
        }

        try
        {
            var (status, patientId) = await _authService.RegisterPatientAsync(
                sName, sBirthDate, sEmail, sPassword, Phone, Gender, Address);

            if (status == 0)
            {
                ErrorMessage = "Email already exists. Please choose a different one.";
            }
            else if (status == 1)
            {
                HttpContext.Session.SetInt32("UserId", patientId);
                HttpContext.Session.SetInt32("UserType", (int)UserType.Patient);
                return RedirectToPage("/Patient/PatientHome");
            }
            else
            {
                ErrorMessage = "An error occurred during registration. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during patient registration for {Email}", sEmail);
            ErrorMessage = "An unexpected error occurred. Please try again.";
        }

        return Page();
    }
}
