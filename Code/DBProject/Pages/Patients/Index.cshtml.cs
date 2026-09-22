using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Patients;

public class IndexModel : PageModel
{
    private readonly IPatientService _patientService;

    public IndexModel(IPatientService patientService)
    {
        _patientService = patientService;
    }

    public IReadOnlyList<PatientDto> Patients { get; private set; } = Array.Empty<PatientDto>();
    [BindProperty(SupportsGet = true)] public string Search { get; set; } = string.Empty;

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Patients = string.IsNullOrWhiteSpace(Search)
            ? await _patientService.GetAllAsync(cancellationToken)
            : await _patientService.SearchAsync(Search, cancellationToken);
    }
}
