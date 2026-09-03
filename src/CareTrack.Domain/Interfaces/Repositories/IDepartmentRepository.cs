using CareTrack.Domain.Entities;

namespace CareTrack.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Department data access.
/// </summary>
public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
