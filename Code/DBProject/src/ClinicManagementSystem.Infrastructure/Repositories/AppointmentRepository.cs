using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Infrastructure.Repositories;

public sealed class AppointmentRepository : IAppointmentRepository
{
    private readonly ClinicManagementDbContext _dbContext;

    public AppointmentRepository(ClinicManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken cancellationToken) =>
        await _dbContext.Appointments.AsNoTracking().Include(x => x.Patient).Include(x => x.Doctor).Where(x => x.IsActive).ToListAsync(cancellationToken);

    public Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.Appointments.Include(x => x.Patient).Include(x => x.Doctor).FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);

    public async Task<Appointment> AddAsync(Appointment entity, CancellationToken cancellationToken)
    {
        _dbContext.Appointments.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Appointment entity, CancellationToken cancellationToken)
    {
        _dbContext.Appointments.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Appointments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return;
        entity.IsActive = false;
        entity.ModifiedDate = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.Appointments.AnyAsync(x => x.Id == id && x.IsActive, cancellationToken);

    public async Task<IReadOnlyList<Appointment>> SearchAsync(string searchTerm, CancellationToken cancellationToken) =>
        await _dbContext.Appointments.AsNoTracking().Include(x => x.Patient).Include(x => x.Doctor)
            .Where(x => x.IsActive && (x.Name.Contains(searchTerm) || x.Status.Contains(searchTerm)))
            .ToListAsync(cancellationToken);
}
