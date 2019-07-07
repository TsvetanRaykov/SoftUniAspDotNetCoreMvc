using System.Linq;

namespace Vxp.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Vxp.Common;
    using Vxp.Data.Models;

    public class UsersSeeder : ISeeder
    {
        private const string InitialAdministratorPassword = "Nescafe3v1";

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var administrator = new ApplicationUser
            {
                Email = "tsvetan.raykov@gmail.com",
                UserName = "tsvetan.raykov@gmail.com",
            };

            var admins = await userManager.GetUsersInRoleAsync(GlobalConstants.Roles.AdministratorRoleName);

            if (admins.Any(a => a.UserName == administrator.UserName))
            {
                return;
            }

            await userManager.CreateAsync(administrator, InitialAdministratorPassword);
            await userManager.AddToRoleAsync(administrator, GlobalConstants.Roles.AdministratorRoleName);
        }
    }
}