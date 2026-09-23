using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Departments;

public class CreateModel : PageModel
{
    private readonly IDepartmentService _service;
    public CreateModel(IDepartmentService service) => _service = service;

    [BindProperty]
    public DepartmentInputModel Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        await _service.CreateAsync(new DepartmentCreateDto(Input.Name, Input.Description), cancellationToken);
        return RedirectToPage("Index");
    }

    public class DepartmentInputModel
    {
        [Required] public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
