using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Appointments;

public sealed class IndexModel : PageModel
{
    private readonly IAppointmentService _service;
    public IReadOnlyList<AppointmentDto> Items { get; private set; } = [];
    public IndexModel(IAppointmentService service) => _service = service;
    public async Task OnGetAsync(CancellationToken cancellationToken) => Items = await _service.GetAllAsync(cancellationToken);
}
