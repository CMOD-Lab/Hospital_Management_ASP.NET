using CareTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareTrack.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for the LoginTable entity.
/// Configured for PostgreSQL compatibility with snake_case naming.
/// </summary>
public class LoginTableConfiguration : IEntityTypeConfiguration<LoginTable>
{
    public void Configure(EntityTypeBuilder<LoginTable> builder)
    {
        builder.ToTable("login_table");
        builder.HasKey(l => l.LoginId);
        builder.Property(l => l.LoginId).HasColumnName("login_id").ValueGeneratedOnAdd();
        builder.Property(l => l.Password).HasMaxLength(20).IsRequired();
        builder.Property(l => l.Email).HasMaxLength(30).IsRequired();
        builder.HasIndex(l => l.Email).IsUnique();
        builder.Property(l => l.Type).IsRequired();
    }
}

/// <summary>
/// EF Core configuration for the Patient entity.
/// Configured for PostgreSQL compatibility with snake_case naming.
/// </summary>
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patient");
        builder.HasKey(p => p.PatientId);
        builder.Property(p => p.PatientId).HasColumnName("patient_id").ValueGeneratedNever();
        builder.Property(p => p.Name).HasMaxLength(30).IsRequired();
        builder.Property(p => p.Phone).HasMaxLength(11);
        builder.Property(p => p.Address).HasMaxLength(40);
        builder.Property(p => p.BirthDate).HasColumnType("date").IsRequired();
        builder.Property(p => p.Gender).HasMaxLength(1).IsRequired();

        builder.HasOne(p => p.Login)
            .WithOne(l => l.Patient)
            .HasForeignKey<Patient>(p => p.PatientId);
    }
}

/// <summary>
/// EF Core configuration for the Department entity.
/// Configured for PostgreSQL compatibility with snake_case naming.
/// </summary>
public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("department");
        builder.HasKey(d => d.DeptNo);
        builder.Property(d => d.DeptNo).HasColumnName("dept_no").ValueGeneratedNever();
        builder.Property(d => d.DeptName).HasColumnName("dept_name").HasMaxLength(30).IsRequired();
        builder.HasIndex(d => d.DeptName).IsUnique();
        builder.Property(d => d.Description).HasMaxLength(1000);
    }
}

/// <summary>
/// EF Core configuration for the Doctor entity.
/// Configured for PostgreSQL compatibility with snake_case naming.
/// </summary>
public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("doctor");
        builder.HasKey(d => d.DoctorId);
        builder.Property(d => d.DoctorId).HasColumnName("doctor_id").ValueGeneratedNever();
        builder.Property(d => d.Name).HasMaxLength(30).IsRequired();
        builder.Property(d => d.Phone).HasMaxLength(11);
        builder.Property(d => d.Address).HasMaxLength(40);
        builder.Property(d => d.BirthDate).HasColumnType("date").IsRequired();
        builder.Property(d => d.Gender).HasMaxLength(1).IsRequired();
        builder.Property(d => d.ChargesPerVisit).HasColumnName("charges_per_visit").IsRequired();
        builder.Property(d => d.MonthlySalary).HasColumnName("monthly_salary");
        builder.Property(d => d.ReputeIndex).HasColumnName("repute_index");
        builder.Property(d => d.PatientsTreated).HasColumnName("patients_treated").HasDefaultValue(0).IsRequired();
        builder.Property(d => d.Qualification).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Specialization).HasMaxLength(100);
        builder.Property(d => d.WorkExperience).HasColumnName("work_experience");
        builder.Property(d => d.Status).IsRequired();

        builder.HasOne(d => d.Department)
            .WithMany(dept => dept.Doctors)
            .HasForeignKey(d => d.DeptNo);

        builder.HasOne(d => d.Login)
            .WithOne(l => l.Doctor)
            .HasForeignKey<Doctor>(d => d.DoctorId);
    }
}

/// <summary>
/// EF Core configuration for the OtherStaff entity.
/// Configured for PostgreSQL compatibility with snake_case naming.
/// </summary>
public class OtherStaffConfiguration : IEntityTypeConfiguration<OtherStaff>
{
    public void Configure(EntityTypeBuilder<OtherStaff> builder)
    {
        builder.ToTable("other_staff");
        builder.HasKey(s => s.StaffId);
        builder.Property(s => s.StaffId).HasColumnName("staff_id").ValueGeneratedOnAdd();
        builder.Property(s => s.Name).HasMaxLength(30).IsRequired();
        builder.Property(s => s.Phone).HasMaxLength(11);
        builder.Property(s => s.Address).HasMaxLength(30);
        builder.Property(s => s.Designation).HasMaxLength(15).IsRequired();
        builder.Property(s => s.Gender).HasMaxLength(1).IsRequired();
        builder.Property(s => s.HighestQualification).HasColumnName("highest_qualification").HasMaxLength(50);
    }
}

/// <summary>
/// EF Core configuration for the Appointment entity.
/// Configured for PostgreSQL compatibility with snake_case naming.
/// </summary>
public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointment");
        builder.HasKey(a => a.AppointId);
        builder.Property(a => a.AppointId).HasColumnName("appoint_id").ValueGeneratedOnAdd();
        builder.Property(a => a.AppointmentStatus).HasColumnName("appointment_status");
        builder.Property(a => a.BillAmount).HasColumnName("bill_amount");
        builder.Property(a => a.BillStatus).HasColumnName("bill_status").HasMaxLength(10);
        builder.Property(a => a.Disease).HasMaxLength(100);
        builder.Property(a => a.Progress).HasMaxLength(100);
        builder.Property(a => a.Prescription).HasMaxLength(100);
        builder.Property(a => a.Date).HasColumnType("timestamp with time zone");

        builder.HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
