using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class HistoryUpdateModel : PageModel
{
    private readonly IDoctorService _doctorService;
    public HistoryUpdateModel(IDoctorService doctorService) => _doctorService = doctorService;
    public int AppointmentId { get; set; }
    public string Disease { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public IActionResult OnGet(int appointmentId)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 2) return RedirectToPage("/SignUp");
        AppointmentId = appointmentId;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || userId == null) return RedirectToPage("/SignUp");

        AppointmentId = appointmentId;
        Disease = disease;
        Progress = progress;
        Prescription = prescription;

        bool success = await _doctorService.UpdatePrescriptionAsync(userId.Value, appointmentId, disease, progress, prescription, cancellationToken);
        Message = success ? "Prescription updated successfully!" : "Failed to update prescription.";
        IsSuccess = success;
        return Page();
    }
}
