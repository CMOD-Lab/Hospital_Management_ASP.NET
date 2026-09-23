using ClinicManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Infrastructure.Data;

public sealed class ClinicManagementDbContext : IdentityDbContext
{
    public ClinicManagementDbContext(DbContextOptions<ClinicManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<Feedback> FeedbackEntries => Set<Feedback>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ClinicManagementDbContext).Assembly);
    }
}
