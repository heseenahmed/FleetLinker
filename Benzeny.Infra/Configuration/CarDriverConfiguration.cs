
using BenzenyMain.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BenzenyMain.Infra.Configuration
{
    public class CarDriverConfiguration : IEntityTypeConfiguration<CarDriver>
    {
        public void Configure(EntityTypeBuilder<CarDriver> builder)
        {
            // تحديد المفتاح المركب (Composite Key)
            builder.HasKey(cd => new { cd.CarId, cd.DriverId });

            // العلاقة بين CarDriver و Car
            builder.HasOne(cd => cd.Car)
                .WithMany(c => c.CarDrivers)
                .HasForeignKey(cd => cd.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // العلاقة بين CarDriver و Driver
            builder.HasOne(cd => cd.Driver)
                .WithMany(d => d.CarDrivers)
                .HasForeignKey(cd => cd.DriverId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
