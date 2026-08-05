using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class AppointmentTakerModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<AppointmentTakerModel> _logger;

    public AppointmentTakerModel(IPatientService patientService, ILogger<AppointmentTakerModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public IEnumerable<FreeSlotDto> FreeSlots { get; set; } = new List<FreeSlotDto>();
    public string DoctorName { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public async Task<IActionResult> OnGetAsync(int doctorId, CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || userId == null) return RedirectToPage("/SignUp");

        DoctorId = doctorId;
        var profile = await _patientService.GetDoctorProfileAsync(doctorId, cancellationToken);
        DoctorName = profile?.Name ?? "Unknown";
        FreeSlots = await _patientService.GetFreeSlotsAsync(doctorId, userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int doctorId, int freeSlot, CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || userId == null) return RedirectToPage("/SignUp");

        DoctorId = doctorId;
        var result = await _patientService.InsertAppointmentAsync(doctorId, userId.Value, freeSlot, cancellationToken);
        Message = result.Message;
        IsSuccess = result.Success;

        var profile = await _patientService.GetDoctorProfileAsync(doctorId, cancellationToken);
        DoctorName = profile?.Name ?? "Unknown";
        FreeSlots = await _patientService.GetFreeSlotsAsync(doctorId, userId.Value, cancellationToken);
        return Page();
    }
}
