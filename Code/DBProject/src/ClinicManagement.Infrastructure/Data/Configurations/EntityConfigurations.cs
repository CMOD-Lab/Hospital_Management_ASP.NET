using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for LoginEntry entity.
/// Configured for PostgreSQL with snake_case naming conventions.
/// </summary>
public class LoginEntryConfiguration : IEntityTypeConfiguration<LoginEntry>
{
    public void Configure(EntityTypeBuilder<LoginEntry> builder)
    {
        builder.ToTable("login_table");
        builder.HasKey(e => e.LoginId);
        builder.Property(e => e.LoginId).HasColumnName("login_id").ValueGeneratedOnAdd();
        builder.Property(e => e.Password).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(30).IsRequired();
        builder.HasIndex(e => e.Email).IsUnique();
        builder.Property(e => e.Type).IsRequired();
    }
}

/// <summary>
/// EF Core configuration for Patient entity.
/// Configured for PostgreSQL with snake_case naming conventions.
/// </summary>
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patient");
        builder.HasKey(e => e.PatientId);
        builder.Property(e => e.PatientId).HasColumnName("patient_id").ValueGeneratedNever();
        builder.Property(e => e.Name).HasMaxLength(30).IsRequired();
        // PostgreSQL: use character(11) instead of SQL Server char(11)
        builder.Property(e => e.Phone).HasColumnType("character(11)");
        builder.Property(e => e.Address).HasMaxLength(40);
        // PostgreSQL: use date instead of SQL Server Date
        builder.Property(e => e.BirthDate).HasColumnType("date").IsRequired();
        // PostgreSQL: use character(1) instead of SQL Server char(1)
        builder.Property(e => e.Gender).HasColumnType("character(1)").IsRequired();

        builder.HasOne(e => e.LoginEntry)
            .WithOne(l => l.Patient)
            .HasForeignKey<Patient>(e => e.PatientId);
    }
}

/// <summary>
/// EF Core configuration for Department entity.
/// Configured for PostgreSQL with snake_case naming conventions.
/// </summary>
public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("department");
        builder.HasKey(e => e.DeptNo);
        builder.Property(e => e.DeptNo).ValueGeneratedNever();
        builder.Property(e => e.DeptName).HasMaxLength(30).IsRequired();
        builder.HasIndex(e => e.DeptName).IsUnique();
        builder.Property(e => e.Description).HasMaxLength(1000);
    }
}

/// <summary>
/// EF Core configuration for Doctor entity.
/// Configured for PostgreSQL with snake_case naming conventions.
/// </summary>
public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("doctor");
        builder.HasKey(e => e.DoctorId);
        builder.Property(e => e.DoctorId).HasColumnName("doctor_id").ValueGeneratedNever();
        builder.Property(e => e.Name).HasMaxLength(30).IsRequired();
        // PostgreSQL: use character(11) instead of SQL Server char(11)
        builder.Property(e => e.Phone).HasColumnType("character(11)");
        builder.Property(e => e.Address).HasMaxLength(40);
        // PostgreSQL: use date instead of SQL Server Date
        builder.Property(e => e.BirthDate).HasColumnType("date").IsRequired();
        // PostgreSQL: use character(1) instead of SQL Server char(1)
        builder.Property(e => e.Gender).HasColumnType("character(1)").IsRequired();
        builder.Property(e => e.ChargesPerVisit).HasColumnName("charges_per_visit").IsRequired();
        builder.Property(e => e.WorkExperience).HasColumnName("work_experience");
        builder.Property(e => e.Qualification).HasMaxLength(100);
        builder.Property(e => e.Specialization).HasMaxLength(50);
        builder.Property(e => e.Status).HasDefaultValue(1);

        builder.HasOne(e => e.LoginEntry)
            .WithOne(l => l.Doctor)
            .HasForeignKey<Doctor>(e => e.DoctorId);

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Doctors)
            .HasForeignKey(e => e.DeptNo)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// EF Core configuration for Appointment entity.
/// Configured for PostgreSQL with snake_case naming conventions.
/// </summary>
public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointment");
        builder.HasKey(e => e.AppointmentId);
        builder.Property(e => e.AppointmentId).HasColumnName("appointment_id").ValueGeneratedOnAdd();
        builder.Property(e => e.Timings).HasMaxLength(30);
        builder.Property(e => e.Disease).HasMaxLength(30);
        builder.Property(e => e.Progress).HasMaxLength(50);
        builder.Property(e => e.Prescription).HasMaxLength(60);
        // PostgreSQL: store enum as varchar/text
        builder.Property(e => e.BillStatus).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.Status).HasConversion<int>();
        // PostgreSQL: use timestamp with time zone for DateTime
        builder.Property(e => e.AppointmentDate).HasColumnType("timestamp with time zone");

        builder.HasOne(e => e.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(e => e.DoctorId)
            .HasConstraintName("fk_appointment_doctor")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(e => e.PatientId)
            .HasConstraintName("fk_appointment_patient")
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// EF Core configuration for OtherStaff entity.
/// Configured for PostgreSQL with snake_case naming conventions.
/// </summary>
public class OtherStaffConfiguration : IEntityTypeConfiguration<OtherStaff>
{
    public void Configure(EntityTypeBuilder<OtherStaff> builder)
    {
        builder.ToTable("other_staff");
        builder.HasKey(e => e.StaffId);
        builder.Property(e => e.StaffId).HasColumnName("staff_id").ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasMaxLength(30).IsRequired();
        // PostgreSQL: use character(11) instead of SQL Server char(11)
        builder.Property(e => e.Phone).HasColumnType("character(11)");
        builder.Property(e => e.Address).HasMaxLength(40);
        // PostgreSQL: use date instead of SQL Server Date
        builder.Property(e => e.BirthDate).HasColumnType("date").IsRequired();
        // PostgreSQL: use character(1) instead of SQL Server char(1)
        builder.Property(e => e.Gender).HasColumnType("character(1)").IsRequired();
        builder.Property(e => e.Designation).HasMaxLength(30);
        builder.Property(e => e.Qualification).HasMaxLength(100);
    }
}
