using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Departments;

public sealed class DeleteModel : PageModel
{
    private readonly IDepartmentService _service;
    public DepartmentDto? Item { get; private set; }
    public DeleteModel(IDepartmentService service) => _service = service;
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        Item = await _service.GetByIdAsync(id, cancellationToken);
        return Item is null ? NotFound() : Page();
    }
    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return RedirectToPage("Index");
    }
}
