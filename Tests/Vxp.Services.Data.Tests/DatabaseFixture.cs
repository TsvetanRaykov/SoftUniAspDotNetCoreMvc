using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vxp.Data;
using Vxp.Data.Models;
using Vxp.Services.Data.Tests;
using Xunit;

namespace Vxp.Services.Data.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public ApplicationDbContext DbContext { get; private set; }

        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "DbDistServiceTest")
               .Options;

            this.DbContext = new ApplicationDbContext(options);
            this.AddUsers();
            this.DbContext.SaveChanges();
        }

        private void AddUsers()
        {
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

            this.DbContext.Add(aUser);

        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }
    }
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
