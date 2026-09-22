using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Appointments;

public class DeleteModel : PageModel
{
    private readonly IAppointmentService _service;
    public DeleteModel(IAppointmentService service) => _service = service;
    public AppointmentDto? Appointment { get; private set; }
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        Appointment = await _service.GetByIdAsync(id, cancellationToken);
        return Appointment is null ? NotFound() : Page();
    }
    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return RedirectToPage("Index");
    }
}
