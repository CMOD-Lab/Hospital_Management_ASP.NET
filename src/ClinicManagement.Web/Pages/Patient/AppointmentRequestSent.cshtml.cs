using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Appointment request sent page model</summary>
public class AppointmentRequestSentModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<AppointmentRequestSentModel> _logger;

    public AppointmentRequestSentViewModel RequestModel { get; set; } = new();

    public AppointmentRequestSentModel(IAppointmentService appointmentService, ILogger<AppointmentRequestSentModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int doctorId, int freeSlot, CancellationToken cancellationToken)
    {
        try
        {
            var patientId = HttpContext.Session.GetInt32("idoriginal");
            if (patientId == null) return RedirectToPage("/Index");

            // Store free slot in session
            HttpContext.Session.SetInt32("freeSlot", freeSlot);

            var (success, message) = await _appointmentService.BookAppointmentAsync(
                doctorId, patientId.Value, freeSlot, cancellationToken);

            RequestModel.Success = success;
            RequestModel.Message = message;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment");
            RequestModel.Success = false;
            RequestModel.Message = "There was an error booking the appointment.";
            return Page();
        }
    }
}
