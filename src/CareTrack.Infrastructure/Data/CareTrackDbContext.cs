using CareTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareTrack.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for the CareTrack application.
/// </summary>
public class CareTrackDbContext : DbContext
{
    public CareTrackDbContext(DbContextOptions<CareTrackDbContext> options) : base(options)
    {
    }

    public DbSet<LoginTable> LoginTable { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<OtherStaff> OtherStaff { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CareTrackDbContext).Assembly);
    }
}
