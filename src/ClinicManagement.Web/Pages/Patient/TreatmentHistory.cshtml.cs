using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TreatmentHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<TreatmentHistoryModel> _logger;
    public TreatmentHistoryViewModel TreatmentHistory { get; set; } = new();

    public TreatmentHistoryModel(IPatientService patientService, ILogger<TreatmentHistoryModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var patientId = HttpContext.Session.GetInt32("idoriginal");
            if (patientId == null) return RedirectToPage("/Index");
            var data = await _patientService.GetTreatmentHistoryAsync(patientId.Value, cancellationToken);
            TreatmentHistory = new TreatmentHistoryViewModel
            {
                Count = data.Count,
                Records = data.Records.Select(r => new TreatmentRecordItemViewModel
                {
                    AppointmentId = r.AppointmentId,
                    DoctorName = r.DoctorName,
                    Disease = r.Disease,
                    Prescription = r.Prescription,
                    Progress = r.Progress,
                    AppointmentDate = r.AppointmentDate.ToString("yyyy-MM-dd")
                })
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading treatment history");
            return Page();
        }
    }
}
