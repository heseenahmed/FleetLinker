
using BenzenyMain.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BenzenyMain.Infra.Configuration
{
    public class BranchConfiguration : IEntityTypeConfiguration<Domain.Entity.Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.IBAN).HasMaxLength(50);
            builder.Property(b => b.Address).HasMaxLength(200);

            builder.HasOne(b => b.Company)
                   .WithMany(c => c.Branches)
                   .HasForeignKey(b => b.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
