using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Take appointment page model</summary>
public class TakeAppointmentModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<TakeAppointmentModel> _logger;

    public AppointmentTakerViewModel AppointmentModel { get; set; } = new();

    public TakeAppointmentModel(IAppointmentService appointmentService, ILogger<TakeAppointmentModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int doctorId = 0, CancellationToken cancellationToken = default)
    {
        try
        {
            var patientId = HttpContext.Session.GetInt32("idoriginal");
            if (patientId == null) return RedirectToPage("/Index");

            // Use session dID if doctorId not provided
            if (doctorId == 0)
            {
                doctorId = HttpContext.Session.GetInt32("dID") ?? 0;
            }

            AppointmentModel.DoctorId = doctorId;

            if (doctorId == 0)
            {
                AppointmentModel.Message = "No doctor selected.";
                return Page();
            }

            var slots = await _appointmentService.GetFreeSlotsAsync(doctorId, patientId.Value, cancellationToken);
            var slotList = slots.ToList();

            if (!slotList.Any())
            {
                AppointmentModel.Message = "There is currently no free slot for this doctor.";
            }
            else
            {
                AppointmentModel.Message = $"The following are the {slotList.Count} free slots of this doctor for today:";
                AppointmentModel.FreeSlots = slotList.Select(s => new FreeSlotItemViewModel
                {
                    SlotId = s.SlotId,
                    Timings = s.Timings
                });
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading free slots");
            AppointmentModel.Message = "There was an error retrieving the doctor's free slots.";
            return Page();
        }
    }
}
