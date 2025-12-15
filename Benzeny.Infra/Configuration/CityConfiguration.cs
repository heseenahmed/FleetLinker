
using BenzenyMain.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BenzenyMain.Infra.Configuration
{
    public class CityConfiguration : IEntityTypeConfiguration<Domain.Entity.City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasMany(r => r.Branches)
                   .WithOne(b => b.City)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
