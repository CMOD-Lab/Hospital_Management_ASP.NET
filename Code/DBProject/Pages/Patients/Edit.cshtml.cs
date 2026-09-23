using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Web.Pages.Patients;

public sealed class EditModel : CreateModel
{
    private readonly IPatientService _patientService;
    public EditModel(IPatientService service) : base(service) => _patientService = service;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var item = await _patientService.GetByIdAsync(id, cancellationToken);
        if (item is null) return NotFound();
        Input = new PatientInputModel
        {
            Name = item.Name,
            Email = item.Email,
            PhoneNumber = item.PhoneNumber,
            BirthDate = item.BirthDate,
            Gender = item.Gender,
            Address = item.Address
        };
        return Page();
    }

    public new async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        await _patientService.UpdateAsync(id, new PatientUpdateDto(Input.Name, Input.Email, Input.PhoneNumber, Input.BirthDate, Input.Gender, Input.Address), cancellationToken);
        return RedirectToPage("Index");
    }
}
