using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Patient history (today's appointments) page model (migrated from PatientHistory.aspx.cs).</summary>
public class PatientHistoryModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PatientHistoryModel> _logger;

    public IEnumerable<Appointment> Appointments { get; set; } = new List<Appointment>();

    public PatientHistoryModel(IDoctorService doctorService, ILogger<PatientHistoryModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Index");

        try
        {
            Appointments = await _doctorService.GetTodaysAppointmentsAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading today's appointments for doctor {DoctorId}", userId);
        }

        return Page();
    }
}
