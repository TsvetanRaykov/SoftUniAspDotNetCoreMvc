using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vxp.Common;
using Vxp.Services.Models;
using Vxp.Web.ViewModels.Users;
using Microsoft.Extensions.Configuration;
using Moq;
using Vxp.Services.Data.Projects;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Vxp.Services.Data.Users;
using Vxp.Data.Models;
using Vxp.Data.Repositories;
using Xunit;
namespace Vxp.Services.Data.Tests
{

    [Collection("Database collection")]
    public class UsersServiceTests
    {
        private readonly DatabaseFixture _fixture;
        private readonly IUsersService _usersService;
        public UsersServiceTests(DatabaseFixture fixture)
        {
            this._fixture = fixture;
            this._usersService = this.GetUsersService();


        }

        [Fact]
        public async Task IsRegisteredShouldReturnTrueForExistingUser()
        {
            var registered = await this._usersService.IsRegistered(this._fixture.TestCustomerName);
            Assert.True(registered);
        }

        [Fact]
        public async Task CreateUserWithoutCompanyShouldReturnNewUserId()
        {
            Mapping.Config(
                typeof(ApplicationUser),
                typeof(UserProfileInputModel),
                typeof(UserProfileCompanyInputModel),
                typeof(UserProfileAddressInputModel),
                typeof(UserDto),
                typeof(ProjectDto),
                typeof(FileStoreDto));

            var newUser = new UserProfileInputModel
            {
                UserName = this._fixture.TestCustomerName,
                FirstName = "newUserFirstName",
                LastName = "newUserLastName"
            };

            var result = await this._usersService.CreateUserAsync(newUser, "password", GlobalConstants.Roles.CustomerRoleName);

            Assert.True(Guid.TryParse(result, out var newUserID));

        }

        [Fact]
        public async Task CreateShouldReturnNullOnCreateUserManagerCreateAsyncFail()
        {
            Mapping.Config(
                typeof(ApplicationUser),
                typeof(UserProfileInputModel),
                typeof(UserProfileCompanyInputModel),
                typeof(UserProfileAddressInputModel),
                typeof(UserDto),
                typeof(ProjectDto),
                typeof(FileStoreDto));
            var newUser = new UserProfileInputModel
            {
                UserName = this._fixture.TestCustomerName,
                FirstName = "newUserFirstName",
                LastName = "newUserLastName"
            };
            var fakeUserManager = new Mock<FakeUserManager>();
            var fakeRoleManager = new Mock<FakeRoleManager>();

            var userStore = this._fixture.DbContext.Users;
            var roleStore = this._fixture.DbContext.Roles;

            fakeUserManager.Setup(x => x.Users).Returns(userStore.AsQueryable);
            fakeRoleManager.Setup(x => x.Roles).Returns(roleStore.AsQueryable);

            var appUserRepository = new EfDeletableEntityRepository<ApplicationUser>(this._fixture.DbContext);

            fakeUserManager.Setup(a => a.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((user, pass) => { user.Id = Guid.NewGuid().ToString(); })
                .Returns(Task.FromResult(IdentityResult.Failed())).Verifiable();

            var userService = new UsersService(fakeUserManager.Object, appUserRepository, fakeRoleManager.Object, this.GetFilesService(), this.GetProjectService());

            var result = await userService.CreateUserAsync(newUser, "password", GlobalConstants.Roles.CustomerRoleName);

            Assert.Null(result);

        }

        [Fact]
        public async Task CreateUserWithCompanyShouldReturnNewUserId()
        {
            Mapping.Config(
                typeof(ApplicationUser),
                typeof(UserProfileInputModel),
                typeof(UserProfileCompanyInputModel),
                typeof(UserProfileAddressInputModel),
                typeof(UserDto),
                typeof(ProjectDto),
                typeof(FileStoreDto));

            var newUser = new UserProfileInputModel
            {
                UserName = this._fixture.TestCustomerName,
                FirstName = "newUserFirstName",
                LastName = "newUserLastName",
                Company = new UserProfileCompanyInputModel()
                {
                    Name = "newCompanyName"
                }
            };


            var result = await this._usersService.CreateUserAsync(newUser, "password", GlobalConstants.Roles.CustomerRoleName);

            Assert.True(Guid.TryParse(result, out var newUserID));

        }

        [Fact]
        public async Task PopulateCommonUserModelPropertiesWhenHasNoVendorTest()
        {
            Mapping.Config(typeof(UserProfileInputModel),
                typeof(UserProfileAddressInputModel),
                typeof(UserProfileCompanyInputModel));

            var model = new UserProfileInputModel();

            await this._usersService.PopulateCommonUserModelPropertiesAsync(model);

            Assert.NotNull(model.Company);
            Assert.IsType<UserProfileCompanyInputModel>(model.Company);
            Assert.NotNull(model.ContactAddress);
            Assert.IsType<UserProfileAddressInputModel>(model.ContactAddress);
            Assert.NotNull(model.AvailableCountries);
            Assert.NotNull(model.AvailableDistributors);
            Assert.NotNull(model.AvailableCountries);
            Assert.NotNull(model.AvailableRoles);

        }

        [Fact]
        public async Task PopulateCommonUserModelPropertiesWhenHasHasVendorTest()
        {
            Mapping.Config(typeof(UserProfileInputModel),
                typeof(UserProfileAddressInputModel),
                typeof(UserProfileCompanyInputModel));

            var model = new UserProfileInputModel();

            var fakeUserManager = new Mock<FakeUserManager>();
            var userStore = this._fixture.DbContext.Users;
            fakeUserManager.Setup(x => x.Users).Returns(userStore.AsQueryable);

            var user = await userStore.FirstOrDefaultAsync();

            await fakeUserManager.Object.AddToRolesAsync(user, new[] { GlobalConstants.Roles.VendorRoleName });

            await this._usersService.PopulateCommonUserModelPropertiesAsync(model);

            Assert.NotNull(model.Company);
            Assert.IsType<UserProfileCompanyInputModel>(model.Company);
            Assert.NotNull(model.ContactAddress);
            Assert.IsType<UserProfileAddressInputModel>(model.ContactAddress);
            Assert.NotNull(model.AvailableCountries);
            Assert.NotNull(model.AvailableDistributors);
            Assert.NotNull(model.AvailableCountries);
            Assert.NotNull(model.AvailableRoles);

        }

        [Fact]
        public async Task UpdateUserShouldUpdateUserData()
        {
            Mapping.Config(typeof(UserDto));

            var user = await this._usersService.GetAll<UserDto>().GetAwaiter().GetResult().FirstOrDefaultAsync();
            var oldName = user.UserName;
            user.UserName = "XXX";
            Mapping.Config(typeof(UserDto));
            await this._usersService.UpdateUserAsync(user, new[] { GlobalConstants.Roles.AdministratorRoleName });

            user = await this._usersService.GetAll<UserDto>().GetAwaiter().GetResult().FirstOrDefaultAsync(u => u.Id == user.Id);

            Assert.NotEqual(oldName, user.UserName);
        }

        [Fact]
        public async Task IsRegisteredShouldReturnTrueIfUserExist()
        {
            Mapping.Config(typeof(UserDto));

            var user = await this._usersService.GetAll<UserDto>().GetAwaiter().GetResult().FirstOrDefaultAsync();

            var isRegistered = await this._usersService.IsRegistered(user.UserName);

            Assert.True(isRegistered);
        }


        private IUsersService GetUsersService()
        {
            var fakeUserManager = new Mock<FakeUserManager>();
            var fakeRoleManager = new Mock<FakeRoleManager>();

            var userStore = this._fixture.DbContext.Users;
            var roleStore = this._fixture.DbContext.Roles;

            fakeUserManager.Setup(x => x.Users).Returns(userStore.AsQueryable);
            fakeRoleManager.Setup(x => x.Roles).Returns(roleStore.AsQueryable);

            var appUserRepository = new EfDeletableEntityRepository<ApplicationUser>(this._fixture.DbContext);

            fakeUserManager.Setup(a => a.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((user, pass) => { user.Id = Guid.NewGuid().ToString(); })
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            return new UsersService(fakeUserManager.Object, appUserRepository, fakeRoleManager.Object, this.GetFilesService(), this.GetProjectService());

        }

        private IFilesService GetFilesService()
        {
            var fakeConfiguration = new Mock<IConfiguration>();
            fakeConfiguration
                .SetupGet(m => m[It.Is<string>(s => s == "FileStorage:AppUsers")])
                .Returns("./UserFiles/Tests/");

            var documentsRepository = new EfDeletableEntityRepository<Document>(this._fixture.DbContext);

            return new FilesService(fakeConfiguration.Object, documentsRepository);
        }

        private IProjectsService GetProjectService()
        {
            var projectRepo = new EfDeletableEntityRepository<Project>(this._fixture.DbContext);
            return new ProjectsService(projectRepo);
        }
    }
}