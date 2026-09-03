using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareTrack.Web.Pages.Patient;

/// <summary>
/// Page model for viewing treatment history.
/// </summary>
public class TreatmentHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<TreatmentHistoryModel> _logger;

    public IEnumerable<Appointment> TreatmentHistory { get; set; } = new List<Appointment>();

    public TreatmentHistoryModel(IPatientService patientService, ILogger<TreatmentHistoryModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 1)
        {
            return RedirectToPage("/SignUp");
        }

        try
        {
            TreatmentHistory = await _patientService.GetTreatmentHistoryAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading treatment history for patient ID: {PatientId}", userId);
        }

        return Page();
    }
}
