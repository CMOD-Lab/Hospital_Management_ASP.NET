using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareTrack.Web.Pages.Doctor;

/// <summary>
/// Page model for billing management.
/// </summary>
public class BillModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<BillModel> _logger;

    public IEnumerable<Appointment> BillableAppointments { get; set; } = new List<Appointment>();
    public string? StatusMessage { get; set; }
    public bool IsSuccess { get; set; }

    public BillModel(IDoctorService doctorService, ILogger<BillModel> logger)
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
            BillableAppointments = await _doctorService.GetBillableAppointmentsAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading billable appointments for doctor ID: {DoctorId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostMarkPaidAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 2)
        {
            return RedirectToPage("/SignUp");
        }

        var success = await _doctorService.MarkBillPaidAsync(userId.Value, appointmentId, cancellationToken);
        StatusMessage = success ? "Bill marked as paid." : "Error updating bill status.";
        IsSuccess = success;

        BillableAppointments = await _doctorService.GetBillableAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkUnpaidAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 2)
        {
            return RedirectToPage("/SignUp");
        }

        var success = await _doctorService.MarkBillUnpaidAsync(userId.Value, appointmentId, cancellationToken);
        StatusMessage = success ? "Bill marked as unpaid." : "Error updating bill status.";
        IsSuccess = success;

        BillableAppointments = await _doctorService.GetBillableAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }
}
