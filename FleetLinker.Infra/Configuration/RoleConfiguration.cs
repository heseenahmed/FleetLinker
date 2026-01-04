using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetLinker.Infra.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasData(
                new ApplicationRole
                {
                    Id = "78f3e2f5-8d5a-4b2a-a9f1-0c5e87a4d5b1",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "78f3e2f5-8d5a-4b2a-a9f1-0c5e87a4d5b1"
                },
                new ApplicationRole
                {
                    Id = "b1d4e5f6-c7a8-4b9c-0d1e-2f2a3b4c5d6e",
                    Name = "Visitor",
                    NormalizedName = "VISITOR",
                    ConcurrencyStamp = "b1d4e5f6-c7a8-4b9c-0d1e-2f2a3b4c5d6e"
                },
                new ApplicationRole
                {
                    Id = "d3f6a7b8-e9c0-4d1e-bf2a-4b4c5d6e7f8a",
                    Name = "Equipment owner",
                    NormalizedName = "EQUIPMENT OWNER",
                    ConcurrencyStamp = "d3f6a7b8-e9c0-4d1e-bf2a-4b4c5d6e7f8a"
                },
                new ApplicationRole
                {
                    Id = "e4a7b8c9-f0d1-4e2f-8a3b-5c5d6e7f8a9b",
                    Name = "Supplier",
                    NormalizedName = "SUPPLIER",
                    ConcurrencyStamp = "e4a7b8c9-f0d1-4e2f-8a3b-5c5d6e7f8a9b"
                },
                new ApplicationRole
                {
                    Id = "f5b8c9d0-01e2-4f3a-9b4c-6d6e7f8a9b0c",
                    Name = "Maintenance workshop owner",
                    NormalizedName = "MAINTENANCE WORKSHOP OWNER",
                    ConcurrencyStamp = "f5b8c9d0-01e2-4f3a-9b4c-6d6e7f8a9b0c"
                },
                new ApplicationRole
                {
                    Id = "06c9d0e1-12f3-4a4b-ac5d-7e7f8a9b0c1d",
                    Name = "driver",
                    NormalizedName = "DRIVER",
                    ConcurrencyStamp = "06c9d0e1-12f3-4a4b-ac5d-7e7f8a9b0c1d"
                },
                new ApplicationRole
                {
                    Id = "17d0e1f2-23a4-4b5c-bd6e-8f8a9b0c1d2e",
                    Name = "mechanical",
                    NormalizedName = "MECHANICAL",
                    ConcurrencyStamp = "17d0e1f2-23a4-4b5c-bd6e-8f8a9b0c1d2e"
                }
            );
        }
    }
}
