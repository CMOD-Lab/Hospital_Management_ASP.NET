using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Infrastructure.Repositories;

public sealed class DoctorRepository : IDoctorRepository
{
    private readonly ClinicManagementDbContext _dbContext;

    public DoctorRepository(ClinicManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Doctor>> GetAllAsync(CancellationToken cancellationToken) =>
        await _dbContext.Doctors.AsNoTracking().Include(x => x.Department).Where(x => x.IsActive).ToListAsync(cancellationToken);

    public async Task<Doctor?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        await _dbContext.Doctors.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);

    public async Task<Doctor> AddAsync(Doctor entity, CancellationToken cancellationToken)
    {
        _dbContext.Doctors.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Doctor entity, CancellationToken cancellationToken)
    {
        _dbContext.Doctors.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Doctors.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return;
        entity.IsActive = false;
        entity.ModifiedDate = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.Doctors.AnyAsync(x => x.Id == id && x.IsActive, cancellationToken);

    public async Task<IReadOnlyList<Doctor>> SearchAsync(string searchTerm, CancellationToken cancellationToken) =>
        await _dbContext.Doctors.AsNoTracking().Include(x => x.Department)
            .Where(x => x.IsActive && (x.Name.Contains(searchTerm) || x.Specialization.Contains(searchTerm)))
            .ToListAsync(cancellationToken);
}
