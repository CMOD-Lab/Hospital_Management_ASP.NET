using CareTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareTrack.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for the CareTrack application.
/// Configured for PostgreSQL with snake_case naming conventions.
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

        // Set default schema to "public" for PostgreSQL
        modelBuilder.HasDefaultSchema("public");

        // Configure PostgreSQL extensions
        modelBuilder.HasPostgresExtension("uuid-ossp");

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CareTrackDbContext).Assembly);
    }
}
