using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Appointments;

public sealed class DetailsModel : PageModel
{
    private readonly IAppointmentService _service;
    public AppointmentDto? Item { get; private set; }
    public DetailsModel(IAppointmentService service) => _service = service;
    public async Task OnGetAsync(int id, CancellationToken cancellationToken) => Item = await _service.GetByIdAsync(id, cancellationToken);
}
