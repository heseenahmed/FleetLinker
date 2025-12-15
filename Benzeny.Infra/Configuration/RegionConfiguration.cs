
using BenzenyMain.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace BenzenyMain.Infra.Configuration
{
    public class RegionConfiguration : IEntityTypeConfiguration<Domain.Entity.Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder
            .HasOne(b => b.Region)
            .WithMany(r => r.Branches)
            .HasForeignKey(b => b.RegionId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
