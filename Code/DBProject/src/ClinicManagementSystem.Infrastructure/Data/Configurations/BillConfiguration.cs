using ClinicManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagementSystem.Infrastructure.Data.Configurations;

public sealed class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.ToTable("bills");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Amount).HasColumnType("numeric(18,2)");
        builder.Property(x => x.PaidDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.CreatedDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.ModifiedDate).HasColumnType("timestamp with time zone");
        builder.HasOne(x => x.Appointment)
            .WithOne(x => x.Bill)
            .HasForeignKey<Bill>(x => x.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
