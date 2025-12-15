
using BenzenyMain.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BenzenyMain.Infra.Configuration
{
    public class DriverFundingAssignmentConfiguration : IEntityTypeConfiguration<DriverFundingAssignment>
    {
        public void Configure(EntityTypeBuilder<DriverFundingAssignment> builder)
        {
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            builder.Property(p => p.TransactionType).HasConversion<int>();
            builder.Property(p => p.LimitType).HasConversion<int?>();
            builder.Property(p => p.WeeklyDays).HasConversion<int?>();

            builder.HasOne(p => p.Driver)
             .WithMany(d => d.FundingAssignments) // add this nav on Driver (below)
             .HasForeignKey(p => p.DriverId)
             .OnDelete(DeleteBehavior.Cascade);

            // Ensure only ONE active assignment per driver
            builder.HasIndex(p => new { p.DriverId, p.IsActive })
             .HasFilter("[IsActive] = 1")
             .IsUnique();
        }
    }
}
