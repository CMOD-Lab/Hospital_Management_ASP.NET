using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations;

public class LoginTableConfiguration : IEntityTypeConfiguration<LoginTable>
{
    public void Configure(EntityTypeBuilder<LoginTable> builder)
    {
        builder.ToTable("login_table");
        builder.HasKey(l => l.LoginId);
        builder.Property(l => l.LoginId).HasColumnName("login_id").ValueGeneratedOnAdd();
        builder.Property(l => l.Password).HasColumnType("varchar(20)").IsRequired();
        builder.Property(l => l.Email).HasColumnType("varchar(30)").IsRequired();
        builder.HasIndex(l => l.Email).IsUnique();
        builder.Property(l => l.Type).HasColumnType("integer").IsRequired();
    }
}

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patient");
        builder.HasKey(p => p.PatientId);
        builder.Property(p => p.PatientId).HasColumnName("patient_id").ValueGeneratedNever();
        builder.Property(p => p.Name).HasColumnType("varchar(30)").IsRequired();
        builder.Property(p => p.Phone).HasColumnType("varchar(11)");
        builder.Property(p => p.Address).HasColumnType("varchar(40)");
        builder.Property(p => p.BirthDate).HasColumnType("timestamp without time zone").IsRequired();
        builder.Property(p => p.Gender).HasColumnType("char(1)").IsRequired();

        builder.HasOne(p => p.Login)
               .WithOne(l => l.Patient)
               .HasForeignKey<Patient>(p => p.PatientId);
    }
}

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("department");
        builder.HasKey(d => d.DeptNo);
        builder.Property(d => d.DeptNo).HasColumnName("dept_no").ValueGeneratedNever();
        builder.Property(d => d.DeptName).HasColumnType("varchar(30)").IsRequired();
        builder.HasIndex(d => d.DeptName).IsUnique();
        builder.Property(d => d.Description).HasColumnType("varchar(1000)");
    }
}

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("doctor");
        builder.HasKey(d => d.DoctorId);
        builder.Property(d => d.DoctorId).HasColumnName("doctor_id").ValueGeneratedNever();
        builder.Property(d => d.Name).HasColumnType("varchar(30)").IsRequired();
        builder.Property(d => d.Phone).HasColumnType("varchar(11)");
        builder.Property(d => d.Address).HasColumnType("varchar(40)");
        builder.Property(d => d.BirthDate).HasColumnType("timestamp without time zone").IsRequired();
        builder.Property(d => d.Gender).HasColumnType("char(1)").IsRequired();
        builder.Property(d => d.ChargesPerVisit).HasColumnName("charges_per_visit").HasColumnType("double precision").IsRequired();
        builder.Property(d => d.MonthlySalary).HasColumnName("monthly_salary").HasColumnType("double precision");
        builder.Property(d => d.ReputeIndex).HasColumnName("repute_index").HasColumnType("double precision");
        builder.Property(d => d.PatientsTreated).HasColumnName("patients_treated").HasColumnType("integer").HasDefaultValue(0).IsRequired();
        builder.Property(d => d.Qualification).HasColumnType("varchar(100)").IsRequired();
        builder.Property(d => d.Specialization).HasColumnType("varchar(100)");
        builder.Property(d => d.WorkExperience).HasColumnName("work_experience").HasColumnType("integer");
        builder.Property(d => d.Status).HasColumnType("integer").IsRequired();

        builder.HasOne(d => d.Department)
               .WithMany(dept => dept.Doctors)
               .HasForeignKey(d => d.DeptNo);

        builder.HasOne(d => d.Login)
               .WithOne(l => l.Doctor)
               .HasForeignKey<Doctor>(d => d.DoctorId);
    }
}

public class OtherStaffConfiguration : IEntityTypeConfiguration<OtherStaff>
{
    public void Configure(EntityTypeBuilder<OtherStaff> builder)
    {
        builder.ToTable("other_staff");
        builder.HasKey(s => s.StaffId);
        builder.Property(s => s.StaffId).HasColumnName("staff_id").ValueGeneratedOnAdd();
        builder.Property(s => s.Name).HasColumnType("varchar(30)").IsRequired();
        builder.Property(s => s.Phone).HasColumnType("varchar(11)");
        builder.Property(s => s.Address).HasColumnType("varchar(30)");
        builder.Property(s => s.Designation).HasColumnType("varchar(15)").IsRequired();
        builder.Property(s => s.Gender).HasColumnType("char(1)").IsRequired();
        builder.Property(s => s.HighestQualification).HasColumnName("highest_qualification").HasColumnType("varchar(50)");
        builder.Property(s => s.Salary).HasColumnType("double precision");
    }
}

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointment");
        builder.HasKey(a => a.AppointId);
        builder.Property(a => a.AppointId).HasColumnName("appoint_id").ValueGeneratedOnAdd();
        builder.Property(a => a.DoctorId).HasColumnName("doctor_id").HasColumnType("integer");
        builder.Property(a => a.PatientId).HasColumnName("patient_id").HasColumnType("integer");
        builder.Property(a => a.Date).HasColumnType("timestamp without time zone");
        builder.Property(a => a.AppointmentStatus).HasColumnName("appointment_status").HasColumnType("integer");
        builder.Property(a => a.BillAmount).HasColumnName("bill_amount").HasColumnType("double precision");
        builder.Property(a => a.BillStatus).HasColumnName("bill_status").HasColumnType("varchar(10)");
        builder.Property(a => a.DoctorNotification).HasColumnName("doctor_notification").HasColumnType("integer");
        builder.Property(a => a.PatientNotification).HasColumnName("patient_notification").HasColumnType("integer");
        builder.Property(a => a.FeedbackStatus).HasColumnName("feedback_status").HasColumnType("integer");
        builder.Property(a => a.Disease).HasColumnType("varchar(100)");
        builder.Property(a => a.Progress).HasColumnType("varchar(100)");
        builder.Property(a => a.Prescription).HasColumnType("varchar(100)");

        builder.HasOne(a => a.Doctor)
               .WithMany(d => d.Appointments)
               .HasForeignKey(a => a.DoctorId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.Patient)
               .WithMany(p => p.Appointments)
               .HasForeignKey(a => a.PatientId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
