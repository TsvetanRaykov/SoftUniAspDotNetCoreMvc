using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vxp.Data;
using Vxp.Data.Models;
using Vxp.Data.Repositories;
using Vxp.Services.Data.Users;
using Xunit;

namespace Vxp.Services.Data.Tests
{
    public class DistributorsServiceTests
    {
        [Fact]
        public async Task GenerateNewDistributorKeyShouldReturnGuidString()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "DbDistServiceTest")
                .Options;

            var dbContext = new ApplicationDbContext(options);

            var aUser = new ApplicationUser
            {
                Email = "test@email.com",
                UserName = "test@email.com",
                BankAccounts = new[]
                {
                    new BankAccount {
                        AccountNumber = "1234",
                        BankName = "Fibank",
                        BicCode = "222333df54",
                        SwiftCode = "qweqweqwe",
                    }
                }
            };

            var distUsersRepository = new EfRepository<DistributorUser>(dbContext);
            var distKeysRepository = new EfDeletableEntityRepository<DistributorKey>(dbContext);

            var fakeUserManager = new Mock<FakeUserManager>();

            var userStore = new List<ApplicationUser> { aUser };

            fakeUserManager.Setup(x => x.Users).Returns(userStore.AsQueryable);

            var distributorService = new DistributorsService(distUsersRepository, fakeUserManager.Object, distKeysRepository);

            var newDistributorKey = await distributorService.GenerateNewDistributorKeyAsync(aUser.Email);

            var expectedDistributorKey = distKeysRepository.AllAsNoTracking()
                 .Include(key => key.BankAccount)
                 .FirstOrDefault(k => k.KeyCode.ToString() == newDistributorKey);

            Assert.True(Guid.TryParse(newDistributorKey, out _));
            Assert.Equal(expectedDistributorKey?.BankAccount.AccountNumber, aUser.BankAccounts.First().AccountNumber);

        }

    }
}