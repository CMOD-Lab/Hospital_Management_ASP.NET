using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareTrack.Web.Pages.Doctor;

/// <summary>
/// Page model for viewing patient history.
/// </summary>
public class PatientHistoryModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PatientHistoryModel> _logger;

    public IEnumerable<Appointment> PatientHistory { get; set; } = new List<Appointment>();

    public PatientHistoryModel(IDoctorService doctorService, ILogger<PatientHistoryModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 2)
        {
            return RedirectToPage("/SignUp");
        }

        try
        {
            PatientHistory = await _doctorService.GetPatientHistoryAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient history for doctor ID: {DoctorId}", userId);
        }

        return Page();
    }
}
