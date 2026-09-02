using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Account;

/// <summary>
/// Page model for the sign-up page.
/// </summary>
public class SignUpModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<SignUpModel> _logger;

    public SignUpModel(IAuthService authService, ILogger<SignUpModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [BindProperty]
    public SignUpViewModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public IActionResult OnGet()
    {
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
            var (success, userId, message) = await _authService.RegisterPatientAsync(
                Input.Name,
                Input.BirthDate,
                Input.Email,
                Input.Password,
                Input.PhoneNo,
                Input.Gender,
                Input.Address);

            if (!success)
            {
                ErrorMessage = message;
                return Page();
            }

            SuccessMessage = "Registration successful! You can now log in.";
            _logger.LogInformation("New patient registered with ID: {PatientId}", userId);
            return RedirectToPage("/Account/Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during patient registration");
            ErrorMessage = "An error occurred during registration. Please try again.";
            return Page();
        }
    }
}
