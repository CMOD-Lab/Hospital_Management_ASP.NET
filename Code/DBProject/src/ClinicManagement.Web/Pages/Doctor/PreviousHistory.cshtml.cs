using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Page model for previous patient history.
/// </summary>
public class PreviousHistoryModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<PreviousHistoryModel> _logger;

    public PreviousHistoryModel(IAppointmentService appointmentService, ILogger<PreviousHistoryModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<AppointmentViewModel> History { get; set; } = new List<AppointmentViewModel>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || !userId.HasValue) return RedirectToPage("/Account/Login");

        try
        {
            var history = await _appointmentService.GetHistoryByDoctorAsync(userId.Value);
            History = history.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                PatientName = a.Patient?.Name,
                AppointmentDate = a.AppointmentDate,
                Disease = a.Disease,
                Prescription = a.Prescription,
                BillStatus = a.BillStatus.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading previous history");
        }

        return Page();
    }
}
