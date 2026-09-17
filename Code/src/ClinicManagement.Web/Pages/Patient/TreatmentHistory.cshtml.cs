using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Treatment history page model (migrated from TreatmentHistory.aspx.cs).</summary>
public class TreatmentHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<TreatmentHistoryModel> _logger;

    public IEnumerable<TreatmentHistory> History { get; set; } = new List<TreatmentHistory>();

    public TreatmentHistoryModel(IPatientService patientService, ILogger<TreatmentHistoryModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToPage("/Index");

        try
        {
            History = await _patientService.GetTreatmentHistoryAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading treatment history for patient {PatientId}", userId);
        }

        return Page();
    }
}
