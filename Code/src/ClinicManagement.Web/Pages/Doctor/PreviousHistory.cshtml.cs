using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Previous history page model (migrated from PreviousHistory.aspx.cs).</summary>
public class PreviousHistoryModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PreviousHistoryModel> _logger;

    public IEnumerable<Appointment> History { get; set; } = new List<Appointment>();

    public PreviousHistoryModel(IDoctorService doctorService, ILogger<PreviousHistoryModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Index");

        try
        {
            History = await _doctorService.GetPatientHistoryAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading previous history for doctor {DoctorId}", userId);
        }

        return Page();
    }
}
