using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Patients;

public sealed class DetailsModel : PageModel
{
    private readonly IPatientService _service;
    public PatientDto? Item { get; private set; }
    public DetailsModel(IPatientService service) => _service = service;
    public async Task OnGetAsync(int id, CancellationToken cancellationToken) => Item = await _service.GetByIdAsync(id, cancellationToken);
}
