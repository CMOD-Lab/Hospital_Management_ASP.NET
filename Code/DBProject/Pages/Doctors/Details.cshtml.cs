using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Doctors;

public sealed class DetailsModel : PageModel
{
    private readonly IDoctorService _service;
    public DoctorDto? Item { get; private set; }
    public DetailsModel(IDoctorService service) => _service = service;
    public async Task OnGetAsync(int id, CancellationToken cancellationToken) => Item = await _service.GetByIdAsync(id, cancellationToken);
}
