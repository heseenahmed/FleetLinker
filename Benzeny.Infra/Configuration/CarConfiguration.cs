
using BenzenyMain.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BenzenyMain.Infra.Configuration
{
    public class CarConfiguration : IEntityTypeConfiguration<Domain.Entity.Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder
                .HasOne(c => c.Branch)
                .WithMany(b => b.Cars)
                .HasForeignKey(c => c.BranchId)
                .OnDelete(DeleteBehavior.Restrict); 
            builder
                .HasOne(c => c.CarModel)
                .WithMany(m => m.Cars)                                             
                .HasForeignKey(c => c.CarModelId)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .HasOne(c => c.CarType)
                .WithMany(t => t.Cars)                 
                .HasForeignKey(c => c.CarTypeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .HasOne(c => c.PlateType)
                .WithMany(p => p.Cars)                 
                .HasForeignKey(c => c.PlateTypeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .HasOne(c => c.CarBrand)
                .WithMany(b => b.Cars)                 
                .HasForeignKey(c => c.CarBrandId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
