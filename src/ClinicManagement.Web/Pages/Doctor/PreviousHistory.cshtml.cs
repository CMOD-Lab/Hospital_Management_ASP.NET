using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PreviousHistoryModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PreviousHistoryModel> _logger;
    public PatientHistoryViewModel HistoryModel { get; set; } = new();

    public PreviousHistoryModel(IDoctorService doctorService, ILogger<PreviousHistoryModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var doctorId = HttpContext.Session.GetInt32("idoriginal");
            if (doctorId == null) return RedirectToPage("/Index");
            var history = await _doctorService.GetPatientHistoryAsync(doctorId.Value, cancellationToken);
            HistoryModel.Records = history.Select(h => new PatientHistoryItemViewModel
            {
                AppointmentId = h.AppointmentId,
                PatientName = h.PatientName,
                Disease = h.Disease,
                Prescription = h.Prescription,
                Progress = h.Progress,
                IsPaid = h.IsPaid,
                AppointmentDate = h.AppointmentDate.ToString("yyyy-MM-dd")
            });
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading previous history");
            return Page();
        }
    }
}
