using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Page model for doctor profile view.
/// </summary>
public class DoctorProfileModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorProfileModel> _logger;

    public DoctorProfileModel(IDoctorService doctorService, ILogger<DoctorProfileModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public DoctorProfileViewModel Profile { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int doctorId)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 1) return RedirectToPage("/Account/Login");

        try
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
            if (doctor == null) return NotFound();

            var age = DateTime.Today.Year - doctor.BirthDate.Year;
            if (doctor.BirthDate.Date > DateTime.Today.AddYears(-age)) age--;

            Profile = new DoctorProfileViewModel
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Gender = doctor.Gender.ToString(),
                ChargesPerVisit = doctor.ChargesPerVisit,
                ReputeIndex = doctor.ReputeIndex,
                PatientsTreated = doctor.PatientsTreated,
                Qualification = doctor.Qualification,
                Specialization = doctor.Specialization,
                WorkExperience = doctor.WorkExperience,
                Age = age
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor profile for doctor ID: {DoctorId}", doctorId);
        }

        return Page();
    }
}
