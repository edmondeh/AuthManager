using AuthManager.Application.Constants;
using AuthManager.Application.Enums;
using AuthManager.Application.Interfaces.Shared;
using AuthManager.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthManager.Infrastructure.Identity.Seeds
{
    public static class DefaultAdminUser
    {
        private async static Task SeedClaimsForAdmin(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync("Admin");
            await roleManager.AddPermissionClaim(adminRole, "Dashboard");
            await roleManager.AddPermissionClaim(adminRole, "Users");
            await roleManager.AddPermissionClaim(adminRole, "Roles");
            await roleManager.AddPermissionClaim(adminRole, "Permission");
        }

        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true,
                CreatedOn = DateTime.Now,
                CreatedBy = "Default",
                LastModifiedOn = DateTime.Now,
                LastModifiedBy = "Default"
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Password1.");
                    await userManager.AddToRoleAsync(defaultUser, Roles.User.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
                await roleManager.SeedClaimsForAdmin();
            }
        }
    }
}