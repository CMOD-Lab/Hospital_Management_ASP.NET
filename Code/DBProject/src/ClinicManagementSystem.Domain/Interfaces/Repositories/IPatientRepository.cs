using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Domain.Interfaces.Repositories;

public interface IPatientRepository
{
    Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken);
    Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Patient> AddAsync(Patient entity, CancellationToken cancellationToken);
    Task UpdateAsync(Patient entity, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Patient>> SearchAsync(string searchTerm, CancellationToken cancellationToken);
}
