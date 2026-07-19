using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Pending appointment page model</summary>
public class PendingAppointmentModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PendingAppointmentModel> _logger;

    public PendingAppointmentViewModel PendingModel { get; set; } = new();
    public string? Message { get; set; }

    public PendingAppointmentModel(IDoctorService doctorService, ILogger<PendingAppointmentModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAppointmentsAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int appointmentId, CancellationToken cancellationToken)
    {
        try
        {
            await _doctorService.ApproveAppointmentAsync(appointmentId, cancellationToken);
            Message = $"Appointment {appointmentId} approved successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment: {Id}", appointmentId);
            Message = "There was an error approving the appointment.";
        }
        await LoadAppointmentsAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int appointmentId, CancellationToken cancellationToken)
    {
        try
        {
            await _doctorService.DeleteAppointmentAsync(appointmentId, cancellationToken);
            Message = $"Appointment {appointmentId} deleted.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment: {Id}", appointmentId);
            Message = "There was an error deleting the appointment.";
        }
        await LoadAppointmentsAsync(cancellationToken);
        return Page();
    }

    private async Task LoadAppointmentsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var doctorId = HttpContext.Session.GetInt32("idoriginal");
            if (doctorId == null) return;

            var appointments = await _doctorService.GetPendingAppointmentsAsync(doctorId.Value, cancellationToken);

            // Manual ViewModel mapping
            PendingModel.Appointments = appointments.Select(a => new AppointmentItemViewModel
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
            _logger.LogError(ex, "Error loading pending appointments");
        }
    }
}
