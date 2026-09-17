using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Bills history page model (migrated from BillsHistory.aspx.cs).</summary>
public class BillsHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<BillsHistoryModel> _logger;

    public IEnumerable<Bill> Bills { get; set; } = new List<Bill>();

    public BillsHistoryModel(IPatientService patientService, ILogger<BillsHistoryModel> logger)
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
            Bills = await _patientService.GetBillHistoryAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bills history for patient {PatientId}", userId);
        }

        return Page();
    }
}
