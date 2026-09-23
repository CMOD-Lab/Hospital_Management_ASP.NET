using ClinicManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagementSystem.Infrastructure.Data.Configurations;

public sealed class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.ToTable("feedback");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Comments).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.Rating).HasColumnType("integer");
        builder.Property(x => x.CreatedDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.ModifiedDate).HasColumnType("timestamp with time zone");
        builder.HasOne(x => x.Patient)
            .WithMany(x => x.FeedbackEntries)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Doctor)
            .WithMany()
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
