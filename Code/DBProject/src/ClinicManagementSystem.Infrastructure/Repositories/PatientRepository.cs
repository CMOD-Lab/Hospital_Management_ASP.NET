using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Infrastructure.Repositories;

public class PatientRepository : Repository<Patient>, IPatientRepository
{
    public PatientRepository(ClinicDbContext context) : base(context)
    {
    }

    public override async Task<IReadOnlyList<Patient>> SearchAsync(string query, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Where(x => x.IsActive && (x.Name.Contains(query) || x.Email.Contains(query) || x.PhoneNumber.Contains(query)))
            .ToListAsync(cancellationToken);
}
