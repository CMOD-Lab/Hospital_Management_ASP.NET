using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Departments;

public sealed class DetailsModel : PageModel
{
    private readonly IDepartmentService _service;
    public DepartmentDto? Item { get; private set; }
    public DetailsModel(IDepartmentService service) => _service = service;
    public async Task OnGetAsync(int id, CancellationToken cancellationToken) => Item = await _service.GetByIdAsync(id, cancellationToken);
}
