using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>History update page model</summary>
public class HistoryUpdateModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<HistoryUpdateModel> _logger;

    [BindProperty]
    public HistoryUpdateViewModel UpdateModel { get; set; } = new();
    public string? Message { get; set; }

    public HistoryUpdateModel(IDoctorService doctorService, ILogger<HistoryUpdateModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int appointmentId = 0, CancellationToken cancellationToken = default)
    {
        await LoadTodaysAppointmentsAsync(cancellationToken);
        UpdateModel.AppointmentId = appointmentId;

        if (appointmentId > 0)
        {
            var appt = UpdateModel.TodaysAppointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            if (appt != null)
            {
                UpdateModel.PatientName = appt.PatientName;
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadTodaysAppointmentsAsync(cancellationToken);
            return Page();
        }

        try
        {
            var doctorId = HttpContext.Session.GetInt32("idoriginal");
            if (doctorId == null) return RedirectToPage("/Index");

            var success = await _doctorService.UpdatePrescriptionAsync(
                doctorId.Value,
                UpdateModel.AppointmentId,
                UpdateModel.Disease,
                UpdateModel.Progress,
                UpdateModel.Prescription,
                cancellationToken);

            Message = success ? "Prescription updated successfully!" : "There was an error updating the prescription.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription");
            Message = "There was an error. Please try again.";
        }

        await LoadTodaysAppointmentsAsync(cancellationToken);
        return Page();
    }

    private async Task LoadTodaysAppointmentsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var doctorId = HttpContext.Session.GetInt32("idoriginal");
            if (doctorId == null) return;

            var appointments = await _doctorService.GetTodaysAppointmentsAsync(doctorId.Value, cancellationToken);
            UpdateModel.TodaysAppointments = appointments.Select(a => new AppointmentItemViewModel
            {
                AppointmentId = a.AppointmentId,
                PatientName = a.PatientName,
                Timings = a.Timings,
                Status = a.Status,
                AppointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading today's appointments");
        }
    }
}
