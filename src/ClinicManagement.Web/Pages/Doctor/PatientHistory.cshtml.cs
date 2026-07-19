using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Patient history page model</summary>
public class PatientHistoryModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PatientHistoryModel> _logger;

    public PatientHistoryViewModel HistoryModel { get; set; } = new();

    public PatientHistoryModel(IDoctorService doctorService, ILogger<PatientHistoryModel> logger)
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

            // Manual ViewModel mapping
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
            _logger.LogError(ex, "Error loading patient history");
            return Page();
        }
    }
}
