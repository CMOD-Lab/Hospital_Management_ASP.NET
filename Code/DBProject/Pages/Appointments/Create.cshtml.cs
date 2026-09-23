using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Appointments;

public class CreateModel : PageModel
{
    private readonly IAppointmentService _service;
    public CreateModel(IAppointmentService service) => _service = service;

    [BindProperty]
    public AppointmentInputModel Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        await _service.CreateAsync(new AppointmentCreateDto(Input.Name, Input.PatientId, Input.DoctorId, Input.ScheduledAt, Input.Status, Input.Prescription, Input.ProgressNotes, Input.Disease), cancellationToken);
        return RedirectToPage("Index");
    }

    public class AppointmentInputModel
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Range(1, int.MaxValue)] public int PatientId { get; set; }
        [Range(1, int.MaxValue)] public int DoctorId { get; set; }
        public DateTime ScheduledAt { get; set; } = DateTime.UtcNow;
        [Required] public string Status { get; set; } = "Pending";
        public string? Prescription { get; set; }
        public string? ProgressNotes { get; set; }
        public string? Disease { get; set; }
    }
}
