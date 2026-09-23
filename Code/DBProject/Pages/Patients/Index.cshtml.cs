using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Patients;

public sealed class IndexModel : PageModel
{
    private readonly IPatientService _service;
    public IReadOnlyList<PatientDto> Items { get; private set; } = [];
    public IndexModel(IPatientService service) => _service = service;
    public async Task OnGetAsync(CancellationToken cancellationToken) => Items = await _service.GetAllAsync(cancellationToken);
}
