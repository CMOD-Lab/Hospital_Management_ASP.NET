using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Infrastructure.Repositories;

public sealed class DepartmentRepository : IDepartmentRepository
{
    private readonly ClinicManagementDbContext _dbContext;

    public DepartmentRepository(ClinicManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken) =>
        await _dbContext.Departments.AsNoTracking().Where(x => x.IsActive).ToListAsync(cancellationToken);

    public Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.Departments.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);

    public async Task<Department> AddAsync(Department entity, CancellationToken cancellationToken)
    {
        _dbContext.Departments.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Department entity, CancellationToken cancellationToken)
    {
        _dbContext.Departments.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Departments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return;
        entity.IsActive = false;
        entity.ModifiedDate = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.Departments.AnyAsync(x => x.Id == id && x.IsActive, cancellationToken);

    public async Task<IReadOnlyList<Department>> SearchAsync(string searchTerm, CancellationToken cancellationToken) =>
        await _dbContext.Departments.AsNoTracking()
            .Where(x => x.IsActive && x.Name.Contains(searchTerm))
            .ToListAsync(cancellationToken);
}
