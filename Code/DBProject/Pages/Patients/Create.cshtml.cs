using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Patients;

public sealed class CreateModel : PageModel
{
    private readonly IPatientService _service;
    public CreateModel(IPatientService service) => _service = service;

    [BindProperty]
    public PatientInputModel Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        var dto = new PatientCreateDto(Input.Name, Input.Email, Input.PhoneNumber, Input.BirthDate, Input.Gender, Input.Address);
        await _service.CreateAsync(dto, cancellationToken);
        return RedirectToPage("Index");
    }

    public sealed class PatientInputModel
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        [DataType(DataType.Date)] public DateTime BirthDate { get; set; } = DateTime.UtcNow.Date;
        [Required] public string Gender { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;
    }
}
