using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vxp.Data;
using Vxp.Data.Models;
using Vxp.Data.Repositories;
using Vxp.Services.Data.Users;
using Xunit;

namespace Vxp.Services.Data.Tests
{
    [Collection("Database collection")]
    public class DistributorsServiceTests
    {
        private readonly DatabaseFixture _fixture;

        public DistributorsServiceTests(DatabaseFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public async Task GenerateNewDistributorKeyShouldReturnGuidString()
        {
            //Arrange
           
            //var fakeUserManager = new Mock<FakeUserManager>();
            //var userStore = new List<ApplicationUser> { aUser };
            //fakeUserManager.Setup(x => x.Users).Returns(userStore.AsQueryable);

            var distUserDistributor = new EfRepository<DistributorUser>(_fixture.DbContext);
            var distKeysRepository = new EfDeletableEntityRepository<DistributorKey>(_fixture.DbContext);
            var appUserRepository = new EfDeletableEntityRepository<ApplicationUser>(_fixture.DbContext);

            var distributorService = new DistributorsService(appUserRepository, distUserDistributor, distKeysRepository);
            var aUser = await appUserRepository.All().FirstAsync();

            //Act
            var newDistributorKey = await distributorService.GenerateNewDistributorKeyAsync(aUser.Email);

            var expectedDistributorKey = distKeysRepository.AllAsNoTracking()
                 .Include(key => key.BankAccount)
                 .FirstOrDefault(k => k.KeyCode.ToString() == newDistributorKey);

            //Assert
            Assert.True(Guid.TryParse(newDistributorKey, out _));
            Assert.Equal(expectedDistributorKey?.BankAccount.AccountNumber, aUser.BankAccounts.First().AccountNumber);

        }

    }
}