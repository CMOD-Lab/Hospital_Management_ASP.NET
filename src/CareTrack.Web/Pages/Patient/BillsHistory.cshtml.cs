using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareTrack.Web.Pages.Patient;

/// <summary>
/// Page model for viewing bills history.
/// </summary>
public class BillsHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<BillsHistoryModel> _logger;

    public IEnumerable<Appointment> BillHistory { get; set; } = new List<Appointment>();

    public BillsHistoryModel(IPatientService patientService, ILogger<BillsHistoryModel> logger)
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
            BillHistory = await _patientService.GetBillHistoryAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bill history for patient ID: {PatientId}", userId);
        }

        return Page();
    }
}
