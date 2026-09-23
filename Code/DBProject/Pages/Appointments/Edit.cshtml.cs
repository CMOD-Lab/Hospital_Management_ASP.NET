using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Web.Pages.Appointments;

public sealed class EditModel : CreateModel
{
    private readonly IAppointmentService _appointmentService;
    public EditModel(IAppointmentService service) : base(service) => _appointmentService = service;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var item = await _appointmentService.GetByIdAsync(id, cancellationToken);
        if (item is null) return NotFound();
        Input = new AppointmentInputModel
        {
            Name = item.Name,
            PatientId = item.PatientId,
            DoctorId = item.DoctorId,
            ScheduledAt = item.ScheduledAt,
            Status = item.Status,
            Prescription = item.Prescription,
            ProgressNotes = item.ProgressNotes,
            Disease = item.Disease
        };
        return Page();
    }

    public new async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        await _appointmentService.UpdateAsync(id, new AppointmentUpdateDto(Input.Name, Input.PatientId, Input.DoctorId, Input.ScheduledAt, Input.Status, Input.Prescription, Input.ProgressNotes, Input.Disease), cancellationToken);
        return RedirectToPage("Index");
    }
}
