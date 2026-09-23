using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Infrastructure.Repositories;

public sealed class PatientRepository : IPatientRepository
{
    private readonly ClinicManagementDbContext _dbContext;

    public PatientRepository(ClinicManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken) =>
        await _dbContext.Patients.AsNoTracking().Where(x => x.IsActive).ToListAsync(cancellationToken);

    public Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.Patients.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);

    public async Task<Patient> AddAsync(Patient entity, CancellationToken cancellationToken)
    {
        _dbContext.Patients.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Patient entity, CancellationToken cancellationToken)
    {
        _dbContext.Patients.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Patients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return;
        entity.IsActive = false;
        entity.ModifiedDate = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.Patients.AnyAsync(x => x.Id == id && x.IsActive, cancellationToken);

    public async Task<IReadOnlyList<Patient>> SearchAsync(string searchTerm, CancellationToken cancellationToken) =>
        await _dbContext.Patients.AsNoTracking()
            .Where(x => x.IsActive && (x.Name.Contains(searchTerm) || x.Email.Contains(searchTerm)))
            .ToListAsync(cancellationToken);
}
