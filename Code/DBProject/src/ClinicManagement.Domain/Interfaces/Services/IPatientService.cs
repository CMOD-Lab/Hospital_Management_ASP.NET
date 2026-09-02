using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for patient operations.
/// </summary>
public interface IPatientService
{
    Task<Patient?> GetPatientByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Patient>> GetAllPatientsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Patient>> SearchPatientsAsync(string searchQuery, CancellationToken cancellationToken = default);
}
