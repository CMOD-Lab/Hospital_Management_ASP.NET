using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Doctors;

public sealed class CreateModel : PageModel
{
    private readonly IDoctorService _service;
    public CreateModel(IDoctorService service) => _service = service;

    [BindProperty]
    public DoctorInputModel Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        var dto = new DoctorCreateDto(Input.Name, Input.Email, Input.PhoneNumber, Input.Gender, Input.Qualification, Input.Specialization, Input.Address, Input.ChargesPerVisit, Input.ExperienceYears, Input.DepartmentId);
        await _service.CreateAsync(dto, cancellationToken);
        return RedirectToPage("Index");
    }

    public sealed class DoctorInputModel
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        [Required] public string Gender { get; set; } = string.Empty;
        [Required] public string Qualification { get; set; } = string.Empty;
        [Required] public string Specialization { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;
        [Range(0, 100000)] public decimal ChargesPerVisit { get; set; }
        [Range(0, 80)] public int ExperienceYears { get; set; }
        [Range(1, int.MaxValue)] public int DepartmentId { get; set; }
    }
}
