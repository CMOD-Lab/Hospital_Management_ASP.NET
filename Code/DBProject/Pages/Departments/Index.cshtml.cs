using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Departments;

public sealed class IndexModel : PageModel
{
    private readonly IDepartmentService _service;
    public IReadOnlyList<DepartmentDto> Items { get; private set; } = [];
    public IndexModel(IDepartmentService service) => _service = service;
    public async Task OnGetAsync(CancellationToken cancellationToken) => Items = await _service.GetAllAsync(cancellationToken);
}
