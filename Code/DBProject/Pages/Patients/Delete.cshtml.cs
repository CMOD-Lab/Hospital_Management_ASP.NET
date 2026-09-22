using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Patients;

public class DeleteModel : PageModel
{
    private readonly IPatientService _patientService;
    public DeleteModel(IPatientService patientService) => _patientService = patientService;
    public PatientDto? Patient { get; private set; }
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        Patient = await _patientService.GetByIdAsync(id, cancellationToken);
        return Patient is null ? NotFound() : Page();
    }
    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        await _patientService.DeleteAsync(id, cancellationToken);
        return RedirectToPage("Index");
    }
}
