using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Page model for the patient home page.
/// </summary>
public class PatientHomeModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientHomeModel> _logger;

    public PatientHomeModel(IPatientService patientService, ILogger<PatientHomeModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public PatientHomeViewModel PatientInfo { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userType != 1 || !userId.HasValue)
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            var patient = await _patientService.GetPatientByIdAsync(userId.Value);
            if (patient == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var age = DateTime.Today.Year - patient.BirthDate.Year;
            if (patient.BirthDate.Date > DateTime.Today.AddYears(-age)) age--;

            PatientInfo = new PatientHomeViewModel
            {
                PatientId = patient.PatientId,
                Name = patient.Name,
                Phone = patient.Phone,
                Address = patient.Address,
                BirthDate = patient.BirthDate.ToString("yyyy-MM-dd"),
                Age = age,
                Gender = patient.Gender.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient home for patient ID: {PatientId}", userId);
        }

        return Page();
    }
}
