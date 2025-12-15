
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace FleetLinker.Domain.Entity
{
    public static class RolesWithClaims
    {
        //public static async Task SeedRolesWithClaims(IServiceProvider serviceProvider)
        //{
        //    var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        //    // Create Admin Role
        //    if (!await roleManager.RoleExistsAsync("Admin"))
        //    {
        //        var adminRole = new ApplicationRole("Admin");
        //        await roleManager.CreateAsync(adminRole);
        //        await roleManager.AddClaimAsync(adminRole, new Claim("Permission", "CanManageUsers"));
        //    }

        //    // Create Client Role
        //    if (!await roleManager.RoleExistsAsync("Client"))
        //    {
        //        var clientRole = new ApplicationRole("Client");
        //        await roleManager.CreateAsync(editorRole);
        //        await roleManager.AddClaimAsync(editorRole, new Claim("Permission", "CanPublishContent"));
        //    }
        //}
    }
}
