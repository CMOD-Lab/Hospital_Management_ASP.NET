using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Appointments;

public class IndexModel : PageModel
{
    private readonly IAppointmentService _service;
    public IndexModel(IAppointmentService service) => _service = service;
    public IReadOnlyList<AppointmentDto> Appointments { get; private set; } = Array.Empty<AppointmentDto>();
    [BindProperty(SupportsGet = true)] public string Search { get; set; } = string.Empty;
    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Appointments = string.IsNullOrWhiteSpace(Search) ? await _service.GetAllAsync(cancellationToken) : await _service.SearchAsync(Search, cancellationToken);
    }
}
