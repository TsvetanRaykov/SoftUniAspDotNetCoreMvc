using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Vxp.Data.Models;

namespace Vxp.Data.Seeding
{
    public class CountriesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Countries.Any())
            {
                return;
            }

            object[] countries = {
                new Country{Name = "Bulgaria", Language = "bg"},
                new Country{Name = "Japan", Language = "jp"}
            };

            await dbContext.AddRangeAsync(countries);
        }
    }
}