using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class BillsHistoryModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<BillsHistoryModel> _logger;

    public BillsHistoryModel(IAppointmentService appointmentService, ILogger<BillsHistoryModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<AppointmentViewModel> Bills { get; set; } = new List<AppointmentViewModel>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || !userId.HasValue) return RedirectToPage("/Account/Login");

        try
        {
            var bills = await _appointmentService.GetBillHistoryByPatientAsync(userId.Value);
            Bills = bills.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                DoctorName = a.Doctor?.Name,
                AppointmentDate = a.AppointmentDate,
                BillStatus = a.BillStatus.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bills history");
        }

        return Page();
    }
}
