using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service implementation for patient operations.
/// </summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets a patient by their ID.
    /// </summary>
    public async Task<Patient?> GetPatientByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving patient with ID: {PatientId}", id);
            return await _patientRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient with ID: {PatientId}", id);
            return null;
        }
    }

    /// <summary>
    /// Gets all patients.
    /// </summary>
    public async Task<IEnumerable<Patient>> GetAllPatientsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all patients");
            return await _patientRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all patients");
            return Enumerable.Empty<Patient>();
        }
    }

    /// <summary>
    /// Searches patients by name.
    /// </summary>
    public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching patients with query: {Query}", searchQuery);
            return await _patientRepository.SearchAsync(searchQuery, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patients with query: {Query}", searchQuery);
            return Enumerable.Empty<Patient>();
        }
    }
}
