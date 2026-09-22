using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Infrastructure.Repositories;

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ClinicDbContext context) : base(context)
    {
    }

    public override async Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().Where(x => x.IsActive)
            .OrderBy(x => x.ScheduledAt)
            .ToListAsync(cancellationToken);

    public override async Task<IReadOnlyList<Appointment>> SearchAsync(string query, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().Where(x => x.IsActive && (x.Status.Contains(query) || x.Notes.Contains(query)))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Appointment>> GetByDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().Where(x => x.IsActive && x.DoctorId == doctorId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Appointment>> GetByPatientAsync(int patientId, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().Where(x => x.IsActive && x.PatientId == patientId).ToListAsync(cancellationToken);
}
