
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Benzeny.Domain.Entity
{
    public static class RolesWithClaims
    {
        //public static async Task SeedRolesWithClaims(IServiceProvider serviceProvider)
        //{
        //    var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        //    // Create Admin Role
        //    if (!await roleManager.RoleExistsAsync("Administrator"))
        //    {
        //        var adminRole = new ApplicationRole("Administrator");
        //        await roleManager.CreateAsync(adminRole);
        //        await roleManager.AddClaimAsync(adminRole, new Claim("Permission", "CanManageUsers"));
        //    }

        //    // Create Editor Role
        //    if (!await roleManager.RoleExistsAsync("Customer Support"))
        //    {
        //        var editorRole = new ApplicationRole("Customer Support");
        //        await roleManager.CreateAsync(editorRole);
        //        await roleManager.AddClaimAsync(editorRole, new Claim("Permission", "CanPublishContent"));
        //    }
        //}
    }
}
