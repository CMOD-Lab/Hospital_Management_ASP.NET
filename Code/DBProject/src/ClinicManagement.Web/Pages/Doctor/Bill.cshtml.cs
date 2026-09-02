using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Page model for bill generation.
/// </summary>
public class BillModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<BillModel> _logger;

    public BillModel(IAppointmentService appointmentService, ILogger<BillModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<AppointmentViewModel> Appointments { get; set; } = new List<AppointmentViewModel>();
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || !userId.HasValue) return RedirectToPage("/Account/Login");

        try
        {
            var appointments = await _appointmentService.GetTodaysAppointmentsByDoctorAsync(userId.Value);
            Appointments = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                PatientName = a.Patient?.Name,
                AppointmentDate = a.AppointmentDate,
                BillStatus = a.BillStatus.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bills");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostMarkPaidAsync(int appointmentId)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || !userId.HasValue) return RedirectToPage("/Account/Login");

        await _appointmentService.MarkBillPaidAsync(userId.Value, appointmentId);
        Message = "Bill marked as paid.";
        return await OnGetAsync();
    }

    public async Task<IActionResult> OnPostMarkUnpaidAsync(int appointmentId)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || !userId.HasValue) return RedirectToPage("/Account/Login");

        await _appointmentService.MarkBillUnpaidAsync(userId.Value, appointmentId);
        Message = "Bill marked as unpaid.";
        return await OnGetAsync();
    }
}
