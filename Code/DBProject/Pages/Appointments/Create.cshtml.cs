using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Appointments;

public class CreateModel : PageModel
{
    private readonly IAppointmentService _service;
    public CreateModel(IAppointmentService service) => _service = service;
    [BindProperty] public AppointmentInputModel Input { get; set; } = new() { ScheduledAt = DateTime.UtcNow.AddDays(1) };
    public void OnGet() { }
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        await _service.CreateAsync(new AppointmentCreateDto(Input.PatientId, Input.DoctorId, Input.ScheduledAt, Input.Notes), cancellationToken);
        return RedirectToPage("Index");
    }
    public class AppointmentInputModel
    {
        [Range(1, int.MaxValue)] public int PatientId { get; set; }
        [Range(1, int.MaxValue)] public int DoctorId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
