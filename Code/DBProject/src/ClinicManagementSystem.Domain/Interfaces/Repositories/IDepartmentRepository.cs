using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Domain.Interfaces.Repositories;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken);
    Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Department> AddAsync(Department entity, CancellationToken cancellationToken);
    Task UpdateAsync(Department entity, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Department>> SearchAsync(string searchTerm, CancellationToken cancellationToken);
}
