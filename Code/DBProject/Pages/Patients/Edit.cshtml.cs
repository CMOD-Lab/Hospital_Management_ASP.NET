using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Patients;

public class EditModel : PageModel
{
    private readonly IPatientService _patientService;
    public EditModel(IPatientService patientService) => _patientService = patientService;
    [BindProperty] public PatientInputModel Input { get; set; } = new();
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var patient = await _patientService.GetByIdAsync(id, cancellationToken);
        if (patient is null) return NotFound();
        Input = new PatientInputModel { Id = patient.Id, Name = patient.Name, Email = patient.Email, PhoneNumber = patient.PhoneNumber, Gender = patient.Gender, Address = patient.Address, BirthDate = patient.BirthDate, IsActive = true };
        return Page();
    }
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        await _patientService.UpdateAsync(Input.Id, new PatientUpdateDto(Input.Name, Input.Email, Input.PhoneNumber, Input.Gender, Input.Address, Input.BirthDate, Input.IsActive), cancellationToken);
        return RedirectToPage("Index");
    }
    public class PatientInputModel
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        [Required] public string Gender { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; }
    }
}
