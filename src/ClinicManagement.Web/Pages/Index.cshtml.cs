using ClinicManagement.Domain.Enums;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages;

/// <summary>Index page model - handles login and signup</summary>
public class IndexModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<IndexModel> _logger;

    [BindProperty]
    public LoginViewModel LoginModel { get; set; } = new();

    [BindProperty]
    public SignUpViewModel SignUpModel { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public IndexModel(IAuthService authService, ILogger<IndexModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public void OnGet()
    {
        // Clear session on page load
        HttpContext.Session.Clear();
    }

    /// <summary>Handles login form submission</summary>
    public async Task<IActionResult> OnPostLoginAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var (success, userId, userType, message) = await _authService.LoginAsync(
                LoginModel.Email, LoginModel.Password, cancellationToken);

            if (success)
            {
                HttpContext.Session.SetInt32("idoriginal", userId);
                HttpContext.Session.SetInt32("userType", (int)userType);

                return userType switch
                {
                    UserType.Patient => RedirectToPage("/Patient/PatientHome"),
                    UserType.Doctor => RedirectToPage("/Doctor/DoctorHome"),
                    UserType.Admin => RedirectToPage("/Admin/AdminHome"),
                    _ => RedirectToPage("/Index")
                };
            }

            ErrorMessage = message;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            ErrorMessage = "There was some error. Try Again!";
            return Page();
        }
    }

    /// <summary>Handles signup form submission</summary>
    public async Task<IActionResult> OnPostSignUpAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var (success, patientId, message) = await _authService.RegisterPatientAsync(
                SignUpModel.Name,
                SignUpModel.BirthDate,
                SignUpModel.Email,
                SignUpModel.Password,
                SignUpModel.Phone,
                SignUpModel.Gender,
                SignUpModel.Address,
                cancellationToken);

            if (success)
            {
                HttpContext.Session.SetInt32("idoriginal", patientId);
                HttpContext.Session.SetInt32("userType", (int)UserType.Patient);
                return RedirectToPage("/Patient/PatientHome");
            }

            ErrorMessage = message;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during patient registration");
            ErrorMessage = "There was some error. Try again!";
            return Page();
        }
    }
}
