using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Patients;

public class CreateModel : PageModel
{
    private readonly IPatientService _patientService;
    public CreateModel(IPatientService patientService) => _patientService = patientService;
    [BindProperty] public PatientInputModel Input { get; set; } = new();
    public void OnGet() { }
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        await _patientService.CreateAsync(new PatientCreateDto(Input.Name, Input.Email, Input.PhoneNumber, Input.Gender, Input.Address, Input.BirthDate), cancellationToken);
        return RedirectToPage("Index");
    }
    public class PatientInputModel
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        [Required] public string Gender { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;
        [DataType(DataType.Date)] public DateTime BirthDate { get; set; } = DateTime.UtcNow.Date;
    }
}
