using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Doctors;

public sealed class IndexModel : PageModel
{
    private readonly IDoctorService _service;
    public IReadOnlyList<DoctorDto> Items { get; private set; } = [];
    public IndexModel(IDoctorService service) => _service = service;
    public async Task OnGetAsync(CancellationToken cancellationToken) => Items = await _service.GetAllAsync(cancellationToken);
}
