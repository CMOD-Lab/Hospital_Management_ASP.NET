using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Doctor home page model</summary>
public class DoctorHomeModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorHomeModel> _logger;

    public DoctorHomeViewModel DoctorInfo { get; set; } = new();

    public DoctorHomeModel(IDoctorService doctorService, ILogger<DoctorHomeModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var doctorId = HttpContext.Session.GetInt32("idoriginal");
            if (doctorId == null) return RedirectToPage("/Index");

            var doctor = await _doctorService.GetByIdAsync(doctorId.Value, cancellationToken);
            if (doctor == null)
            {
                return RedirectToPage("/Index");
            }

            // Manual ViewModel mapping from DTO
            DoctorInfo = new DoctorHomeViewModel
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Email = doctor.Email,
                Phone = doctor.Phone,
                Gender = doctor.Gender,
                DepartmentName = doctor.DepartmentName,
                ChargesPerVisit = doctor.ChargesPerVisit,
                ReputeIndex = doctor.ReputeIndex,
                PatientsTreated = doctor.PatientsTreated,
                Qualification = doctor.Qualification,
                Specialization = doctor.Specialization,
                Experience = doctor.Experience,
                Age = doctor.Age
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor home");
            return Page();
        }
    }
}
