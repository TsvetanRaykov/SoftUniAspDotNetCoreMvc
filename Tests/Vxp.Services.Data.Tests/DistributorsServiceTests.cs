using Xunit;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vxp.Data.Models;
using Vxp.Data.Repositories;
using Vxp.Services.Data.BankAccounts;
using Vxp.Services.Data.Users;
using Vxp.Services.Messaging;
using Vxp.Services.Models;
using Vxp.Web.ViewModels.Distributor.Customers;
[assembly: CollectionBehavior(MaxParallelThreads = 1)]

namespace Vxp.Services.Data.Tests
{


    [Collection("Database collection")]
   
    public class DistributorsServiceTests
    {

        private readonly DatabaseFixture _fixture;
        private readonly IDistributorsService _distributorsService;

        public DistributorsServiceTests(DatabaseFixture fixture)
        {
            this._fixture = fixture;
            this._distributorsService = this.GetDistributorsService();
        }

        [Fact]
        public async Task GenerateNewDistributorKeyShouldReturnGuidString()
        {
            //Arrange
            var appUserRepository = new EfDeletableEntityRepository<ApplicationUser>(this._fixture.DbContext);
            var aUser = await appUserRepository.All().FirstAsync(u => u.BankAccounts.Any());
            //Act
            var newDistributorKey = this._distributorsService.GenerateNewKeyForDistributor(aUser.Email);
            //Assert
            Assert.True(Guid.TryParse(newDistributorKey, out _));
        }

        [Fact]
        public async Task GenerateNewDistributorKeyShouldReturnBullWhenNoBankAccount()
        {
            //Arrange
            var appUserRepository = new EfDeletableEntityRepository<ApplicationUser>(this._fixture.DbContext);
            var aUser = await appUserRepository.All().FirstAsync(u => !u.BankAccounts.Any());
            //Act
            var newDistributorKey = this._distributorsService.GenerateNewKeyForDistributor(aUser.Email);

            //Assert
            Assert.Null(newDistributorKey);

        }

        [Fact]
        public async Task AddCustomerToDistributorShouldCreateDistributorUser()
        {
            var appUserRepository = new EfDeletableEntityRepository<ApplicationUser>(this._fixture.DbContext);
            var distributor = await appUserRepository.All().FirstAsync(u => u.BankAccounts.Any());
            var customer = await appUserRepository.All().FirstAsync(u => !u.BankAccounts.Any());
            var distributorKey = this._distributorsService.GenerateNewKeyForDistributor(distributor.Email);

            var bankAccountRepo = new EfDeletableEntityRepository<BankAccount>(this._fixture.DbContext);
            var fakeBankAccountService = new BankAccountsService(bankAccountRepo);

            var bankAccount = await bankAccountRepo.AllAsNoTracking()
                .FirstOrDefaultAsync(b => b.OwnerId == distributor.Id);

            var addKeyToBankAccount = await fakeBankAccountService.AssignDistributorKeyToBankAccountAsync(bankAccount.Id, distributorKey);
            var addCustomerToDistributor = await this._distributorsService.AddCustomerToDistributorAsync(customer.UserName, distributorKey);

            var distUserDistributor = new EfRepository<DistributorUser>(this._fixture.DbContext);
            var distributorUser = await distUserDistributor
                .AllAsNoTracking()
                .Include(k => k.DistributorKey)
                .FirstOrDefaultAsync(k => k.DistributorKey.KeyCode.ToString() == distributorKey);

            Assert.NotNull(distributorUser);

        }

        [Fact]
        public async Task AddCustomerToDistributorShouldReturnFalseWhenNoUser()
        {
            var appUserRepository = new EfDeletableEntityRepository<ApplicationUser>(this._fixture.DbContext);
            var user = await appUserRepository.AllAsNoTracking().FirstOrDefaultAsync();

            var result1 = await this._distributorsService.AddCustomerToDistributorAsync(user.FirstName, null);
            var result2 = await this._distributorsService.AddCustomerToDistributorAsync(null, this._fixture.TestDistributorKey);

            Assert.False(result1);
            Assert.False(result2);
        }

        [Fact]
        public async Task RemoveCustomerFromDistributorShouldRemoveKey()
        {
            Mapping.Config(typeof(UserDto));

            var customer = await this._distributorsService.GetCustomersAsync<UserDto>(this._fixture.TestDistributorName)
                .GetAwaiter().GetResult().FirstOrDefaultAsync();

            var removeRelation = await this._distributorsService.RemoveCustomerFromDistributorAsync(customer.UserName,
                 this._fixture.TestDistributorName);

            customer = await this._distributorsService.GetCustomersAsync<UserDto>(this._fixture.TestDistributorName)
               .GetAwaiter().GetResult().FirstOrDefaultAsync();

            Assert.Null(customer);
        }

        [Fact]
        public async Task SendSuccessfullyInvitationToCustomerShouldReturnTrue()
        {
            Mapping.Config(typeof(UserDto), typeof(EmailDto), typeof(CustomerInvitation));

            var distributor = await this._distributorsService.GetDistributorByKey<UserDto>(
                new Guid(this._fixture.TestDistributorKey));

            var newMail = new EmailDto
            {
                DistributorKey = this._fixture.TestDistributorKey,
                EmailTo = this._fixture.TestDistributorName,
                MessageBody = "Body",
                Subject = "Subject"
            };

            var result = await this._distributorsService.SendInvitationToCustomerAsync(newMail, distributor.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task GetCustomerInvitationsShouldReturnCorrectCount()
        {
            Mapping.Config(typeof(UserDto), typeof(EmailDto), typeof(CustomersInvitationInputModel));

            var distributor = await this._distributorsService.GetDistributorByKey<UserDto>(
                new Guid(this._fixture.TestDistributorKey));

            var newMail = new EmailDto
            {
                DistributorKey = this._fixture.TestDistributorKey,
                EmailTo = this._fixture.TestDistributorName,
                MessageBody = "Body",
                Subject = "Subject"
            };

            var sendInvitation = await this._distributorsService.SendInvitationToCustomerAsync(newMail, distributor.Id);

            var result = await this._distributorsService.GetCustomerInvitationsAsync<CustomersInvitationInputModel>(distributor.Id);

            Assert.Collection(result, item =>
                 Assert.Contains(this._fixture.TestDistributorKey, item.DistributorKey));
            Assert.Single(result);
        }


        [Fact]
        public async Task SendInvitationShouldThrowExceptionOnFailure()
        {
            var result = await this._distributorsService
                .SendInvitationToCustomerAsync(null, Guid.Empty.ToString());

            Assert.False(result);
        }

        [Fact]
        public async Task GetDistributorByKeyShouldReturnCorrectUser()
        {
            Mapping.Config(typeof(UserDto));

            var distributor =
                await this._distributorsService.GetDistributorByKey<UserDto>(
                    new Guid(this._fixture.TestDistributorKey));

            Assert.True(distributor.UserName == this._fixture.TestDistributorName);
        }

        [Fact]
        public async Task GetDistributorByKeyShouldReturnNullIfNotFound()
        {
            Mapping.Config(typeof(UserDto));

            var distributor =
                await this._distributorsService.GetDistributorByKey<UserDto>(Guid.NewGuid());

            Assert.Null(distributor);
        }

        [Fact]
        public async Task GetDistributorForUsersShouldReturnCorrectCollection()
        {
            Mapping.Config(typeof(UserDto));

            var distributors =
                await this._distributorsService.GetDistributorsForUserAsync<UserDto>(this._fixture.TestCustomerName)
                    .GetAwaiter().GetResult().ToListAsync();
            Assert.Collection(distributors,
                item => Assert.Contains(this._fixture.TestDistributorName, item.UserName));
            Assert.Single(distributors);
        }

        private IDistributorsService GetDistributorsService()
        {
            var fakeUserManager = new Mock<FakeUserManager>();

            var userStore = this._fixture.DbContext.Users;

            fakeUserManager.Setup(x => x.Users).Returns(userStore.AsQueryable);

            var distUserDistributor = new EfRepository<DistributorUser>(this._fixture.DbContext);
            var distKeysRepository = new EfDeletableEntityRepository<DistributorKey>(this._fixture.DbContext);
            var appUserRepository = new EfDeletableEntityRepository<ApplicationUser>(this._fixture.DbContext);
            var customerInvitationsRepository = new EfDeletableEntityRepository<CustomerInvitation>(this._fixture.DbContext);

            return new DistributorsService(appUserRepository, distUserDistributor, distKeysRepository, fakeUserManager.Object, new NullMessageSender(), customerInvitationsRepository);

        }
    }
}