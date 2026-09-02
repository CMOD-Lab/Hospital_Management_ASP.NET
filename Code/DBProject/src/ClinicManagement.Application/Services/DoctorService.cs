using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service implementation for doctor operations.
/// </summary>
public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly ILoginRepository _loginRepository;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(
        IDoctorRepository doctorRepository,
        ILoginRepository loginRepository,
        ILogger<DoctorService> logger)
    {
        _doctorRepository = doctorRepository;
        _loginRepository = loginRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets a doctor by their ID.
    /// </summary>
    public async Task<Doctor?> GetDoctorByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctor with ID: {DoctorId}", id);
            return await _doctorRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor with ID: {DoctorId}", id);
            return null;
        }
    }

    /// <summary>
    /// Gets all active doctors.
    /// </summary>
    public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all doctors");
            return await _doctorRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all doctors");
            return Enumerable.Empty<Doctor>();
        }
    }

    /// <summary>
    /// Searches doctors by name.
    /// </summary>
    public async Task<IEnumerable<Doctor>> SearchDoctorsAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching doctors with query: {Query}", searchQuery);
            return await _doctorRepository.SearchAsync(searchQuery, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching doctors with query: {Query}", searchQuery);
            return Enumerable.Empty<Doctor>();
        }
    }

    /// <summary>
    /// Gets doctors by department name.
    /// </summary>
    public async Task<IEnumerable<Doctor>> GetDoctorsByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctors for department: {Department}", departmentName);
            return await _doctorRepository.GetByDepartmentAsync(departmentName, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors for department: {Department}", departmentName);
            return Enumerable.Empty<Doctor>();
        }
    }

    /// <summary>
    /// Adds a new doctor with login credentials.
    /// </summary>
    public async Task<bool> AddDoctorAsync(Doctor doctor, string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new doctor: {DoctorName}", doctor.Name);

            // Create login entry
            var loginEntry = new LoginEntry
            {
                Email = email,
                Password = password,
                Type = (int)UserType.Doctor
            };
            var createdLogin = await _loginRepository.AddAsync(loginEntry, cancellationToken);

            // Set doctor ID from login
            doctor.DoctorId = createdLogin.LoginId;
            await _doctorRepository.AddAsync(doctor, cancellationToken);

            _logger.LogInformation("Doctor added successfully with ID: {DoctorId}", doctor.DoctorId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor: {DoctorName}", doctor.Name);
            return false;
        }
    }

    /// <summary>
    /// Soft-deletes a doctor (sets status to 0).
    /// </summary>
    public async Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting doctor with ID: {DoctorId}", id);
            await _doctorRepository.DeleteAsync(id, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor with ID: {DoctorId}", id);
            return false;
        }
    }

    /// <summary>
    /// Checks if a doctor email already exists.
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _doctorRepository.EmailExistsAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email existence: {Email}", email);
            return false;
        }
    }
}
