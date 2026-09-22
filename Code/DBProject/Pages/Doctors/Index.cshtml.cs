using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Doctors;

public class IndexModel : PageModel
{
    private readonly IDoctorService _doctorService;

    public IndexModel(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public IReadOnlyList<DoctorDto> Doctors { get; private set; } = Array.Empty<DoctorDto>();

    [BindProperty(SupportsGet = true)]
    public string Search { get; set; } = string.Empty;

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Doctors = string.IsNullOrWhiteSpace(Search)
            ? await _doctorService.GetAllAsync(cancellationToken)
            : await _doctorService.SearchAsync(Search, cancellationToken);
    }
}
