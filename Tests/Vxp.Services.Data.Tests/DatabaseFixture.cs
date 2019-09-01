using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using Vxp.Common;
using Vxp.Data;
using Vxp.Data.Models;
using Vxp.Services.Data.Tests;
using Xunit;

namespace Vxp.Services.Data.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public readonly string TestDistributorKey = "5f9005bb-6b53-445a-ae73-f22e7df5e349";
        public readonly string TestDistributorName = "distributor@email.com";
        public readonly string TestCustomerName = "customer@email.com";

        public ApplicationDbContext DbContext { get; private set; }

        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "DbDistServiceTest")
               .Options;

            this.DbContext = new ApplicationDbContext(options);
            this.AddRoles();
            this.AddUsers();
            this.DbContext.SaveChanges();
        }

        private void AddRoles()
        {
            foreach (FieldInfo info in typeof(GlobalConstants.Roles).GetFields().Where(x => x.IsStatic && x.IsLiteral))
            {
                var role = new ApplicationRole(info.GetValue(null) as string);
                this.DbContext.Add(role);
            }
        }

        private void AddUsers()
        {
            var aUser = new ApplicationUser
            {
                Email = "test@email.com",
                UserName = "test@email.com",
                BankAccounts = new[] {
                    new BankAccount {
                        AccountNumber = "1234",
                        BankName = "Fibank",
                        BicCode = "222333df54",
                        SwiftCode = "qweqweqwe",
                    }
                }
            };

            var aUserWithoutBankAccount = new ApplicationUser
            {
                Email = "nobank@user.com",
                UserName = "nobank@user.com"
            };

            var userDistributor = new ApplicationUser
            {
                Email = TestDistributorName,
                UserName = TestDistributorName,
                BankAccounts = new[] {
                    new BankAccount {
                        AccountNumber = "1234",
                        BankName = "Fibank",
                        BicCode = "222333df54",
                        SwiftCode = "qweqweqwe",
                        DistributorKeys = new[]
                        {
                            new DistributorKey
                            {
                                KeyCode = new Guid(TestDistributorKey)
                            }
                        }
                    }
                }
            };

            var userCustomer = new ApplicationUser
            {
                Email = this.TestCustomerName,
                UserName = this.TestCustomerName
            };

            userCustomer.Distributors.Add(new DistributorUser
            {
                ApplicationUser = userDistributor,
                DistributorKey = userDistributor
                    .BankAccounts.First().DistributorKeys.First()
            });

            this.DbContext.Add(aUser);
            this.DbContext.Add(aUserWithoutBankAccount);
            this.DbContext.Add(userDistributor);
            this.DbContext.Add(userCustomer);

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
