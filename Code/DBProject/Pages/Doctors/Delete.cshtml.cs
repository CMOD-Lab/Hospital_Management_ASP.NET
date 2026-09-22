using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Doctors;

public class DeleteModel : PageModel
{
    private readonly IDoctorService _doctorService;

    public DeleteModel(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public DoctorDto? Doctor { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        Doctor = await _doctorService.GetByIdAsync(id, cancellationToken);
        return Doctor is null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        await _doctorService.DeleteAsync(id, cancellationToken);
        return RedirectToPage("Index");
    }
}
