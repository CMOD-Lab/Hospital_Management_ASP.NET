using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
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

        builder.HasDefaultSchema("public");
        builder.HasPostgresExtension("uuid-ossp");
        builder.ApplyConfigurationsFromAssembly(typeof(ClinicDbContext).Assembly);

        builder.Entity<Department>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        builder.Entity<Doctor>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Qualification).HasMaxLength(200);
            entity.Property(e => e.Specialization).HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Salary).HasColumnType("numeric(18,2)");
            entity.Property(e => e.ChargesPerVisit).HasColumnType("numeric(18,2)");
            entity.Property(e => e.BirthDate).HasColumnType("timestamp with time zone");
            entity.HasOne(e => e.Department)
                .WithMany(d => d.Doctors)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Patient>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.BirthDate).HasColumnType("timestamp with time zone");
        });

        builder.Entity<Appointment>(entity =>
        {
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.ScheduledAt).HasColumnType("timestamp with time zone");
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<TreatmentRecord>(entity =>
        {
            entity.Property(e => e.Disease).HasMaxLength(200);
            entity.Property(e => e.ProgressNotes).HasMaxLength(2000);
            entity.Property(e => e.Prescription).HasMaxLength(2000);
            entity.HasOne(e => e.Appointment)
                .WithOne(a => a.TreatmentRecord)
                .HasForeignKey<TreatmentRecord>(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Bill>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("numeric(18,2)");
            entity.Property(e => e.IssuedOn).HasColumnType("timestamp with time zone");
            entity.HasOne(e => e.Appointment)
                .WithOne(a => a.Bill)
                .HasForeignKey<Bill>(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Feedback>(entity =>
        {
            entity.Property(e => e.Comments).HasMaxLength(2000);
            entity.HasOne(e => e.Appointment)
                .WithOne(a => a.Feedback)
                .HasForeignKey<Feedback>(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable("asp_net_roles");
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("asp_net_users");
        });

        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("asp_net_user_roles");
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("asp_net_user_claims");
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("asp_net_user_logins");
        });

        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("asp_net_role_claims");
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("asp_net_user_tokens");
        });
    }
}
