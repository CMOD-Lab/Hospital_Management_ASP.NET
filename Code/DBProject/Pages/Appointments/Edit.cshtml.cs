using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Appointments;

public class EditModel : PageModel
{
    private readonly IAppointmentService _service;
    public EditModel(IAppointmentService service) => _service = service;
    [BindProperty] public AppointmentInputModel Input { get; set; } = new();
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var appointment = await _service.GetByIdAsync(id, cancellationToken);
        if (appointment is null) return NotFound();
        Input = new AppointmentInputModel { Id = appointment.Id, PatientId = appointment.PatientId, DoctorId = appointment.DoctorId, ScheduledAt = appointment.ScheduledAt, Status = appointment.Status, Notes = appointment.Notes, IsActive = true };
        return Page();
    }
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(Input.Id, new AppointmentUpdateDto(Input.PatientId, Input.DoctorId, Input.ScheduledAt, Input.Status, Input.Notes, Input.IsActive), cancellationToken);
        return RedirectToPage("Index");
    }
    public class AppointmentInputModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
