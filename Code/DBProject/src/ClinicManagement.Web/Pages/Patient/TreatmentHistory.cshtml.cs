using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TreatmentHistoryModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<TreatmentHistoryModel> _logger;

    public TreatmentHistoryModel(IAppointmentService appointmentService, ILogger<TreatmentHistoryModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<AppointmentViewModel> Treatments { get; set; } = new List<AppointmentViewModel>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || !userId.HasValue) return RedirectToPage("/Account/Login");

        try
        {
            var treatments = await _appointmentService.GetTreatmentHistoryByPatientAsync(userId.Value);
            Treatments = treatments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                DoctorName = a.Doctor?.Name,
                AppointmentDate = a.AppointmentDate,
                Disease = a.Disease,
                Progress = a.Progress,
                Prescription = a.Prescription
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading treatment history");
        }

        return Page();
    }
}
