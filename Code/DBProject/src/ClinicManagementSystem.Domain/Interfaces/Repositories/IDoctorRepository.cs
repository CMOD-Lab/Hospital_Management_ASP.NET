using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Domain.Interfaces.Repositories;

public interface IDoctorRepository
{
    Task<IReadOnlyList<Doctor>> GetAllAsync(CancellationToken cancellationToken);
    Task<Doctor?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Doctor> AddAsync(Doctor entity, CancellationToken cancellationToken);
    Task UpdateAsync(Doctor entity, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Doctor>> SearchAsync(string searchTerm, CancellationToken cancellationToken);
}
