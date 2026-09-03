using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareTrack.Web.Pages.Doctor;

/// <summary>
/// Page model for viewing previous patient history.
/// </summary>
public class PreviousHistoryModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PreviousHistoryModel> _logger;

    public IEnumerable<Appointment> CompletedAppointments { get; set; } = new List<Appointment>();

    public PreviousHistoryModel(IDoctorService doctorService, ILogger<PreviousHistoryModel> logger)
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
            var history = await _doctorService.GetPatientHistoryAsync(userId.Value, cancellationToken);
            CompletedAppointments = history.Where(a => a.AppointmentStatus == 3);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading previous history for doctor ID: {DoctorId}", userId);
        }

        return Page();
    }
}
