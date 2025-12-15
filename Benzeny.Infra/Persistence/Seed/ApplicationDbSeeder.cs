
using Benzeny.Domain.Entity;
using Benzeny.Infra.Data;
using BenzenyMain.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BenzenyMain.Infra.Persistence.Seed
{
    public class ApplicationDbSeeder
    {
        public static async Task SeedRolesAndAdminUserAsync(
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager)
        {
            string[] roles = { "Admin", "CompanyOwner", "BranchManager", "Finance", "Driver" , "BSuperAdmin" , "BFinanceAdmin" , "BOperationAdmin" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Id = Guid.NewGuid().ToString(), Name = roleName, NormalizedName = roleName.ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() });
                }
            }

            // Seed Admin user
            string adminEmail = "admin@benzeny.com";
            string adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "System Administrator"
                };

                var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createUserResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception("Failed to create Admin user: " + createUserResult.Errors.FirstOrDefault()?.Description);
                }
            }
        }

        public static async Task SeedRegionsAndCitiesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (await context.Regions.AnyAsync()) return;

            var provinces = new Dictionary<string, List<string>>
            {
                ["منطقة الرياض"] = new() { "الرياض", "الدرعية", "الخرج", "الزلفي", "الدلم", "المجمعة", "عفيف", "وادي الدواسر", "القويعية", "أشيقر", "شقراء", "حريملاء", "المزاحمية", "رماح" },
                ["منطقة مكة المكرمة"] = new() { "مكة المكرمة", "جدة", "الطائف", "القنفذة", "رابغ", "الجموم", "خليص", "الكامل", "الخرمة", "رنية", "تربة" },
                ["المنطقة الشرقية"] = new() { "الدمام", "الخبر", "الظهران", "القطيف", "الاحساء", "حفر الباطن", "جبيل", "الخفجي", "رأس تنورة", "نعرية" },
                ["منطقة القصيم"] = new() { "بريدة", "عنيزة", "الرس", "البكيرية", "غبقة", "عيون الجواء", "عقلة الصقور" },
                ["منطقة حائل"] = new() { "حائل", "بقعاء", " الشنان", "العلايا" },
                ["منطقة عسير"] = new() { "أبها", "خميس مشيط", "النماص", "بيشة", "المجاردة", "بلقرن" },
                ["منطقة جازان"] = new() { "جازان", "صبيا", "أبوعريش", "ضمد", "الدرب", "فرسان", "بيش" },
                ["الحدود الشمالية"] = new() { "عرعر", "رفحاء", "طلعة نمار" },
                ["منطقة الجوف"] = new() { "سكاكا", "القريات", "دومة الجندل" },
                ["منطقة تبوك"] = new() { "تبوك", "ضبا", "أملج", "حقل", "وجه", "تيماء" },
                ["المدينة المنورة"] = new() { "المدينة المنورة", "ينبع", "بدر", "خيبر", "المهد", "العلا" },
                ["منطقة نجران"] = new() { "نجران", "شرورة", "حبونا" },
                ["منطقة الباحة"] = new() { "الباحة", "بلجرشي", "المندق", "العقيق", "قلوة" }
            };

            var regions = provinces.Keys
                .Select(name => new Region { Id = Guid.NewGuid(), Title = name })
                .ToList();

            var cities = new List<City>();
            foreach (var region in regions)
            {
                foreach (var cityName in provinces[region.Title])
                {
                    cities.Add(new City { Id = Guid.NewGuid(), Name = cityName, RegionId = region.Id });
                }
            }

            await context.Regions.AddRangeAsync(regions);
            await context.Cities.AddRangeAsync(cities);
            await context.SaveChangesAsync();
        }
        public static async Task SeedCarDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!await context.CarBrands.AnyAsync())
            {
                await context.CarBrands.AddRangeAsync(new[]
                {
                    new CarBrand { Title = "تويوتا" },
                    new CarBrand { Title = "هيونداي" },
                    new CarBrand { Title = "كيا" }
                });
            }

            if (!await context.CarModels.AnyAsync())
            {
                await context.CarModels.AddRangeAsync(new[]
                {
                    new CarModel { Title = "كورولا" },
                    new CarModel { Title = "سوناتا" },
                    new CarModel { Title = "ريو" }
                });
            }

            if (!await context.CarTypes.AnyAsync())
            {
                await context.CarTypes.AddRangeAsync(new[]
                {
                    new CarType { Title = "سيدان" },
                    new CarType { Title = "هاتشباك" },
                    new CarType { Title = "دفع رباعي" }
                });
            }

            if (!await context.PlateTypes.AnyAsync())
            {
                await context.PlateTypes.AddRangeAsync(new[]
                {
                    new PlateType { Title = "خصوصي" },
                    new PlateType { Title = "نقل" },
                    new PlateType { Title = "أجرة" }
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
