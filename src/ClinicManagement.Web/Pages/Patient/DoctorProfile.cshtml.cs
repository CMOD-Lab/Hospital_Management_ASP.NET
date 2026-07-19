using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Doctor profile page model</summary>
public class DoctorProfileModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorProfileModel> _logger;

    public DoctorProfileViewModel Profile { get; set; } = new();

    public DoctorProfileModel(IDoctorService doctorService, ILogger<DoctorProfileModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int doctorId, CancellationToken cancellationToken)
    {
        try
        {
            // Store doctor ID in session for appointment booking
            HttpContext.Session.SetInt32("dID", doctorId);

            var doctor = await _doctorService.GetByIdAsync(doctorId, cancellationToken);
            if (doctor == null) return RedirectToPage("/Patient/ViewDoctors");

            // Manual ViewModel mapping from DTO
            Profile = new DoctorProfileViewModel
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Gender = doctor.Gender,
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
            _logger.LogError(ex, "Error loading doctor profile for ID: {DoctorId}", doctorId);
            return Page();
        }
    }
}
