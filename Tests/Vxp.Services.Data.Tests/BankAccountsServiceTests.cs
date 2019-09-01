using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vxp.Data.Models;
using Vxp.Data.Repositories;
using Vxp.Services.Data.BankAccounts;
using Vxp.Services.Models;
using Vxp.Web.ViewModels.Users;
using Xunit;

namespace Vxp.Services.Data.Tests
{
    [Collection("Database collection")]
    public class BankAccountsServiceTests
    {
        private readonly DatabaseFixture _fixture;
        private readonly IBankAccountsService _bankAccountsService;

        public BankAccountsServiceTests(DatabaseFixture fixture)
        {
            this._fixture = fixture;
            this._bankAccountsService = this.GetBankAccountsService();
        }

        [Fact]
        public async Task GetBankAccountsForUserShouldReturnCollectionOfBankAccounts()
        {
            Mapping.Config(typeof(UserProfileInputModel));
            var bankAccounts =
                this._bankAccountsService.GetBankAccountsForUser<SelectListItem>(this._fixture.TestDistributorName);
            Assert.Collection(bankAccounts, ba => Assert.True(int.TryParse(ba.Value as string, out var _)));
        }

        [Fact]
        public async Task CreateBankAccountTest()
        {
            Mapping.Config(typeof(UserProfileBankAccountInputModel));
            var user = this._fixture.DbContext.Users.FirstOrDefaultAsync();
            var newBankAccount = new UserProfileBankAccountInputModel
            {
                OwnerId = user.Id.ToString()
            };
            var bankAccount = await this._bankAccountsService.CreateBankAccountAsync(newBankAccount);
            Assert.NotNull(bankAccount.Id);
        }

        [Fact]
        public async Task RemoveBankAccountTest()
        {
            Mapping.Config(typeof(BankAccountDto));
            var bankAccount = this._fixture.DbContext.BankAccounts.FirstOrDefaultAsync();
            int bankAccountId = bankAccount.Id;
            var result = await this._bankAccountsService.RemoveBankAccountAsync(bankAccountId);
            var resultAccount = await this._fixture.DbContext.BankAccounts.FirstOrDefaultAsync(b => b.Id == bankAccountId);
            Assert.True(result);
            Assert.Null(resultAccount);
        }

        [Fact]
        public async Task RemoveBankAccountShouldReturnFalseIfAccountIsNotFound()
        {
            Mapping.Config(typeof(BankAccountDto));
            var result = await this._bankAccountsService.RemoveBankAccountAsync(int.MaxValue);
            Assert.False(result);
        }

        [Fact]
        public async Task GetBankAccountByIdTest()
        {
            Mapping.Config(typeof(BankAccountDto));
            var bankAccountFromDb = await this._fixture.DbContext.BankAccounts.FirstOrDefaultAsync();
            var bankAccountFromService =
                 this._bankAccountsService.GetBankAccountById<BankAccountDto>(bankAccountFromDb.Id);
            Assert.NotNull(bankAccountFromService);
        }

        [Fact]
        public async Task GetBankAccountByIdShouldReturnDefaultIfNotFound()
        {
            Mapping.Config(typeof(BankAccountDto));
            var bankAccountFromService =
                this._bankAccountsService.GetBankAccountById<BankAccountDto>(int.MaxValue);
            Assert.Null(bankAccountFromService);
        }

        [Fact]
        public async Task UpdateBankAccountTest()
        {
            Mapping.Config(typeof(UserProfileBankAccountInputModel));
            var bankAccountToUpdate = await this._bankAccountsService
                .GetBankAccountsForUser<UserProfileBankAccountInputModel>(this._fixture.TestDistributorName).FirstOrDefaultAsync();
            var testNumber = Guid.NewGuid().ToString();
            bankAccountToUpdate.AccountNumber = testNumber;
            await this._bankAccountsService.UpdateBankAccountAsync(bankAccountToUpdate);
            var result = await this._fixture.DbContext.BankAccounts.FirstOrDefaultAsync(b => b.AccountNumber == testNumber);
            Assert.NotNull(result);
        }


        private IBankAccountsService GetBankAccountsService()
        {
            var bankAccountsRepository = new EfDeletableEntityRepository<BankAccount>(this._fixture.DbContext);
            return new BankAccountsService(bankAccountsRepository);
        }
    }
}