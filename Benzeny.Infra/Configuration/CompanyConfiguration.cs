
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BenzenyMain.Infra.Configuration
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Domain.Entity.Company>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.Company> builder)
        {
            // Table Name
            builder.ToTable("Companies");

            // Key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(c => c.CompanyEmail)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(c => c.CompanyPhone)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(c => c.Description)
                   .HasMaxLength(1000);

            builder.Property(c => c.IBAN)
                   .HasMaxLength(50);

            // Complex types (List of FilePaths) - stored as JSON string
            builder.Property(c => c.FilePaths)
                   .HasConversion(
                        v => string.Join(";", v),
                        v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                   )
                   .HasColumnName("FilePaths");

            // Owner Relationship (optional)
            builder.HasOne(c => c.CompanyOwner)
                   .WithMany()
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relationship: One Company → Many Users
            builder.HasMany(c => c.Users)
                   .WithOne(u => u.Company)
                   .HasForeignKey(u => u.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
