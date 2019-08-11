
namespace Vxp.Data.Seeding
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Vxp.Common;
    using Models;

    public class UsersSeeder : ISeeder
    {
        private const string InitialAdministratorPassword = "Nescafe3v1";

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var administrator = new ApplicationUser
            {
                FirstName = "Tsvetan",
                LastName = "Raykov",
                Email = "tsvetan.raykov@gmail.com",
                UserName = "tsvetan.raykov@gmail.com",
                ContactAddress = new Address
                {
                    AddressLocation = "121 Fake Str.",
                    Email = "tsvetan.raykov@gmail.com",
                    City = "Lovech",
                    CountryName = "Bulgaria",
                    Phone = "0889999888"
                }
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