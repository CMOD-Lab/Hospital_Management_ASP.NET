using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Patient home page model</summary>
public class PatientHomeModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientHomeModel> _logger;

    public PatientHomeViewModel PatientInfo { get; set; } = new();

    public PatientHomeModel(IPatientService patientService, ILogger<PatientHomeModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var patientId = HttpContext.Session.GetInt32("idoriginal");
            if (patientId == null) return RedirectToPage("/Index");

            var patient = await _patientService.GetByIdAsync(patientId.Value, cancellationToken);
            if (patient == null) return RedirectToPage("/Index");

            // Manual ViewModel mapping from DTO
            PatientInfo = new PatientHomeViewModel
            {
                Name = patient.Name,
                Phone = patient.Phone,
                BirthDate = patient.BirthDate,
                Age = patient.Age,
                Address = patient.Address,
                Gender = patient.Gender
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient home");
            return Page();
        }
    }
}
