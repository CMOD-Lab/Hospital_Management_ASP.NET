using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Domain.Interfaces.Repositories;

public interface IDoctorRepository : IRepository<Doctor>
{
    Task<IReadOnlyList<Doctor>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);
}
