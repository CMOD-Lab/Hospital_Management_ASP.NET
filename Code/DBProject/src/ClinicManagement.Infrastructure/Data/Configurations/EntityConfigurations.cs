using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for LoginEntry entity.
/// </summary>
public class LoginEntryConfiguration : IEntityTypeConfiguration<LoginEntry>
{
    public void Configure(EntityTypeBuilder<LoginEntry> builder)
    {
        builder.ToTable("LoginTable");
        builder.HasKey(e => e.LoginId);
        builder.Property(e => e.LoginId).HasColumnName("LoginID").ValueGeneratedOnAdd();
        builder.Property(e => e.Password).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(30).IsRequired();
        builder.HasIndex(e => e.Email).IsUnique();
        builder.Property(e => e.Type).IsRequired();
    }
}

/// <summary>
/// EF Core configuration for Patient entity.
/// </summary>
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patient");
        builder.HasKey(e => e.PatientId);
        builder.Property(e => e.PatientId).HasColumnName("PatientID").ValueGeneratedNever();
        builder.Property(e => e.Name).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Phone).HasColumnType("char(11)");
        builder.Property(e => e.Address).HasMaxLength(40);
        builder.Property(e => e.BirthDate).HasColumnType("Date").IsRequired();
        builder.Property(e => e.Gender).HasColumnType("char(1)").IsRequired();

        builder.HasOne(e => e.LoginEntry)
            .WithOne(l => l.Patient)
            .HasForeignKey<Patient>(e => e.PatientId);
    }
}

/// <summary>
/// EF Core configuration for Department entity.
/// </summary>
public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Department");
        builder.HasKey(e => e.DeptNo);
        builder.Property(e => e.DeptNo).ValueGeneratedNever();
        builder.Property(e => e.DeptName).HasMaxLength(30).IsRequired();
        builder.HasIndex(e => e.DeptName).IsUnique();
        builder.Property(e => e.Description).HasMaxLength(1000);
    }
}

/// <summary>
/// EF Core configuration for Doctor entity.
/// </summary>
public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctor");
        builder.HasKey(e => e.DoctorId);
        builder.Property(e => e.DoctorId).HasColumnName("DoctorID").ValueGeneratedNever();
        builder.Property(e => e.Name).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Phone).HasColumnType("char(11)");
        builder.Property(e => e.Address).HasMaxLength(40);
        builder.Property(e => e.BirthDate).HasColumnType("Date").IsRequired();
        builder.Property(e => e.Gender).HasColumnType("char(1)").IsRequired();
        builder.Property(e => e.ChargesPerVisit).HasColumnName("Charges_Per_Visit").IsRequired();
        builder.Property(e => e.WorkExperience).HasColumnName("WorkExperience");
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
/// </summary>
public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointment");
        builder.HasKey(e => e.AppointmentId);
        builder.Property(e => e.AppointmentId).HasColumnName("AppointmentID").ValueGeneratedOnAdd();
        builder.Property(e => e.Timings).HasMaxLength(30);
        builder.Property(e => e.Disease).HasMaxLength(30);
        builder.Property(e => e.Progress).HasMaxLength(50);
        builder.Property(e => e.Prescription).HasMaxLength(60);
        builder.Property(e => e.BillStatus).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.Status).HasConversion<int>();

        builder.HasOne(e => e.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(e => e.DoctorId)
            .HasConstraintName("FK_Appointment_Doctor")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(e => e.PatientId)
            .HasConstraintName("FK_Appointment_Patient")
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// EF Core configuration for OtherStaff entity.
/// </summary>
public class OtherStaffConfiguration : IEntityTypeConfiguration<OtherStaff>
{
    public void Configure(EntityTypeBuilder<OtherStaff> builder)
    {
        builder.ToTable("OtherStaff");
        builder.HasKey(e => e.StaffId);
        builder.Property(e => e.StaffId).HasColumnName("StaffID").ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Phone).HasColumnType("char(11)");
        builder.Property(e => e.Address).HasMaxLength(40);
        builder.Property(e => e.BirthDate).HasColumnType("Date").IsRequired();
        builder.Property(e => e.Gender).HasColumnType("char(1)").IsRequired();
        builder.Property(e => e.Designation).HasMaxLength(30);
        builder.Property(e => e.Qualification).HasMaxLength(100);
    }
}
