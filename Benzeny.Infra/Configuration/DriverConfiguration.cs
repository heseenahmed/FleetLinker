
using BenzenyMain.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace BenzenyMain.Infra.Configuration
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder
                .HasOne(d => d.Tag)
                .WithMany(t => t.Drivers)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
