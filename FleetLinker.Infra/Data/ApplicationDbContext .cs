using FleetLinker.Domain.Entity;
using FleetLinker.Infra.Configuration;
using FleetLinker.Domain.Entity;
using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
namespace FleetLinker.Infra.Data
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser, ApplicationRole, string,
        IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentSparePart> EquipmentSpareParts { get; set; }
        public DbSet<SparePartOffer> SparePartOffers { get; set; }
        public DbSet<EquipmentRequest> EquipmentRequests { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.Entity<ApplicationUserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<ApplicationUserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique()
                .HasFilter("[PhoneNumber] IS NOT NULL");

            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}