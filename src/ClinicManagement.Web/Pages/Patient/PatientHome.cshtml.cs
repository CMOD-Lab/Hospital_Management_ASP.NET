using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Patient home page model.</summary>
public class PatientHomeModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientHomeModel> _logger;

    public PatientHomeModel(IPatientService patientService, ILogger<PatientHomeModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public PatientInfoDto? PatientInfo { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userType != 1 || userId == null) return RedirectToPage("/SignUp");

        try
        {
            PatientInfo = await _patientService.GetPatientInfoAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient home for ID: {UserId}", userId);
        }

        return Page();
    }
}
