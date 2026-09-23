using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Web.Pages.Departments;

public sealed class EditModel : CreateModel
{
    private readonly IDepartmentService _departmentService;
    public EditModel(IDepartmentService service) : base(service) => _departmentService = service;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var item = await _departmentService.GetByIdAsync(id, cancellationToken);
        if (item is null) return NotFound();
        Input = new DepartmentInputModel { Name = item.Name, Description = item.Description };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        await _departmentService.UpdateAsync(id, new DepartmentUpdateDto(Input.Name, Input.Description), cancellationToken);
        return RedirectToPage("Index");
    }
}
