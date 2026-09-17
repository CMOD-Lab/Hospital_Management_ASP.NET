using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Appointment taker page model (migrated from AppointmentTaker.aspx.cs).</summary>
public class AppointmentTakerModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<AppointmentTakerModel> _logger;

    public int DoctorId { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<Appointment> FreeSlots { get; set; } = new List<Appointment>();

    public AppointmentTakerModel(IAppointmentService appointmentService, ILogger<AppointmentTakerModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int doctorId)
    {
        int? patientId = HttpContext.Session.GetInt32("UserId");
        if (patientId == null)
            return RedirectToPage("/Index");

        DoctorId = doctorId;

        try
        {
            FreeSlots = await _appointmentService.GetFreeSlotsAsync(doctorId, patientId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading free slots for doctor {DoctorId}", doctorId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int doctorId, int slotIndex)
    {
        int? patientId = HttpContext.Session.GetInt32("UserId");
        if (patientId == null)
            return RedirectToPage("/Index");

        try
        {
            var (success, message) = await _appointmentService.BookAppointmentAsync(doctorId, patientId.Value, slotIndex);
            if (success)
            {
                return RedirectToPage("/Patient/AppointmentRequestSent");
            }
            else
            {
                Message = $"Failed to book appointment: {message}";
                DoctorId = doctorId;
                FreeSlots = await _appointmentService.GetFreeSlotsAsync(doctorId, patientId.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment");
            Message = "An error occurred while booking the appointment.";
        }

        return Page();
    }
}
