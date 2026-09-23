using ClinicManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagementSystem.Infrastructure.Data.Configurations;

public sealed class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("doctors");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
        builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Gender).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Qualification).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Specialization).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Address).IsRequired().HasMaxLength(250);
        builder.Property(x => x.ChargesPerVisit).HasColumnType("numeric(18,2)");
        builder.Property(x => x.ExperienceYears).HasColumnType("integer");
        builder.Property(x => x.CreatedDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.ModifiedDate).HasColumnType("timestamp with time zone");
        builder.HasOne(x => x.Department)
            .WithMany(x => x.Doctors)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
