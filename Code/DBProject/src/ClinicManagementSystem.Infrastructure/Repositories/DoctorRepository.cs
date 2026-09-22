using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Infrastructure.Repositories;

public class DoctorRepository : Repository<Doctor>, IDoctorRepository
{
    public DoctorRepository(ClinicDbContext context) : base(context)
    {
    }

    public override async Task<IReadOnlyList<Doctor>> SearchAsync(string query, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Where(x => x.IsActive && (x.Name.Contains(query) || x.Specialization.Contains(query) || x.Email.Contains(query)))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Doctor>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().Where(x => x.IsActive && x.DepartmentId == departmentId).ToListAsync(cancellationToken);
}
