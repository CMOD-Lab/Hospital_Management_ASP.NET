using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Infrastructure.Repositories;

public class StaffMemberRepository : Repository<StaffMember>, IStaffMemberRepository
{
    public StaffMemberRepository(ClinicDbContext context) : base(context)
    {
    }

    public override async Task<IReadOnlyList<StaffMember>> SearchAsync(string query, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Where(x => x.IsActive && (x.Name.Contains(query) || x.Designation.Contains(query)))
            .ToListAsync(cancellationToken);
}
