using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Patients;

public class DetailsModel : PageModel
{
    private readonly IPatientService _patientService;
    public DetailsModel(IPatientService patientService) => _patientService = patientService;
    public PatientDto? Patient { get; private set; }
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        Patient = await _patientService.GetByIdAsync(id, cancellationToken);
        return Patient is null ? NotFound() : Page();
    }
}
