using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for department operations.
/// </summary>
public interface IDepartmentService
{
    Task<IEnumerable<Department>> GetAllDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<Department?> GetDepartmentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Department?> GetDepartmentByNameAsync(string name, CancellationToken cancellationToken = default);
}
