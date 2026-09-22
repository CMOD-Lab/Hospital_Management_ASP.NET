using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Infrastructure.Persistence;

public class ClinicDbContext : IdentityDbContext<ApplicationUser>
{
    public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<TreatmentRecord> TreatmentRecords => Set<TreatmentRecord>();
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Feedback> FeedbackEntries => Set<Feedback>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ClinicDbContext).Assembly);
    }
}
