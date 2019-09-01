using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.FileProviders;
using Vxp.Data.Common.Enums;
using Vxp.Services.Data.Projects;
using Vxp.Services.Models;

namespace Vxp.Services.Data.Users
{
    using Mapping;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Vxp.Data.Common.Repositories;
    using Vxp.Data.Models;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.IO;
    using System.Reflection;
    using Common;
    using Vxp.Web.ViewModels.Users;

    public class UsersService : IUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IDeletableEntityRepository<ApplicationUser> _usersRepository;
        private readonly IFilesService _filesService;
        private readonly IProjectsService _projectsService;

        public UsersService(
            UserManager<ApplicationUser> userManager,
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            RoleManager<ApplicationRole> roleManager,
            IFilesService filesService,
            IProjectsService projectsService)
        {
            this._userManager = userManager;
            this._usersRepository = usersRepository;
            this._roleManager = roleManager;
            this._filesService = filesService;
            this._projectsService = projectsService;
        }

        public Task<IQueryable<TViewModel>> GetAll<TViewModel>(Expression<Func<ApplicationUser, bool>> exp)
        {
            return GetAllUsers<TViewModel>(false, exp);
        }

        public Task<IQueryable<TViewModel>> GetAllWithDeletedAsync<TViewModel>(Expression<Func<ApplicationUser, bool>> exp = null)
        {
            return GetAllUsers<TViewModel>(true, exp);
        }

        public Task<IQueryable<TViewModel>> GetAllInRoleAsync<TViewModel>(string roleName)
        {
            var applicationUsersInRole = this._usersRepository.AllAsNoTrackingWithDeleted()
                .Where(u => u.Roles.Any(r => r.Role.Name == roleName));

            return Task.Run(() => applicationUsersInRole.To<TViewModel>());
        }

        public async Task<string> CreateUserAsync<TViewModel>(TViewModel userModel, string password, string role)
        {
            var applicationUser = AutoMapper.Mapper.Map<ApplicationUser>(userModel);

            if (string.IsNullOrWhiteSpace(applicationUser.Company?.Name))
            {
                applicationUser.Company = null;
            }
            else
            {
                if (string.IsNullOrEmpty(applicationUser.Company.ContactAddress?.City))
                {
                    applicationUser.Company.ContactAddress = null;
                }
                if (string.IsNullOrEmpty(applicationUser.Company.ShippingAddress?.City))
                {
                    applicationUser.Company.ShippingAddress = null;
                }
            }

            applicationUser.Email = applicationUser.UserName;

            var user = await this._userManager.CreateAsync(applicationUser, password);

            if (!user.Succeeded) { return null; }

            await this._userManager.AddToRoleAsync(applicationUser, role);

            var vendor = await this.GetAllInRoleAsync<UserDto>(GlobalConstants.Roles.VendorRoleName)
                             .GetAwaiter().GetResult().FirstOrDefaultAsync() ?? AutoMapper.Mapper.Map<UserDto>(applicationUser);

            var defaultProject = new ProjectDto
            {
                OwnerId = applicationUser.Id,
                PartnerId = vendor.Id,
                Name = "Default project",
                Description = "Welcome new user!"
            };

            var defaultProjectId = await this._projectsService.CreateProjectAsync(defaultProject);
            var termsAndConditionsPdf = new FileStoreDto
            {
                Description = "Terms and conditions",
                UserId = applicationUser.Id,
                Type = DocumentType.Contract,
                ContentType = "application/pdf",
                ProjectId = defaultProjectId,
                OriginalFileName = "terms_and_conditions.pdf"
            };

            var newFile = await this._filesService.StoreFileToDatabaseAsync(termsAndConditionsPdf);

            if (!string.IsNullOrWhiteSpace(newFile.Location))
            {
                var assembly = Assembly.GetEntryAssembly();
                try
                {
                    using (var resourceStream = assembly?.GetManifestResourceStream("Vxp.Web.Resources.terms_and_conditions.pdf"))
                    {
                        using (var fileStream = new FileStream(newFile.Location, FileMode.Create, FileAccess.Write))
                        {
                            //if (resourceStream != null)
                            await resourceStream.CopyToAsync(fileStream);
                        }
                    }
                }
                catch
                {
                    await this._filesService.DeleteFileFromDataBaseAsync(newFile.Id);
                }

            }

            return applicationUser.Id;

        }

        public async Task<bool> UpdateUserAsync<TViewModel>(TViewModel userModel, IEnumerable<string> roleNames)
        {
            var applicationUser = AutoMapper.Mapper.Map<ApplicationUser>(userModel);

            var userFromDb = this._usersRepository.AllWithDeleted().FirstOrDefault(u => u.Id == applicationUser.Id);

            if (userFromDb == null)
            {
                return false;
            }

            AutoMapper.Mapper.Map(userModel, userFromDb);

            if (userFromDb.Company?.Name == null)
            {
                userFromDb.Company = null;
            }
            else
            {
                if (userFromDb.Company.ContactAddress.AddressLocation == null)
                {
                    userFromDb.Company.ContactAddress = null;
                }

                if (userFromDb.Company.ShippingAddress.AddressLocation == null)
                {
                    userFromDb.Company.ShippingAddress = null;
                }
            }

            var currentRoles = await this._userManager.GetRolesAsync(userFromDb);
            await this._userManager.RemoveFromRolesAsync(userFromDb, currentRoles);

            foreach (var roleName in roleNames)
            {
                await this._userManager.AddToRoleAsync(userFromDb, roleName);
            }

            this._usersRepository.Update(userFromDb);
            this._usersRepository.SaveChangesAsync().GetAwaiter().GetResult();

            return true;
        }

        public async Task<bool> UpdateUserPasswordAsync(string userId, string password)
        {
            var userFromDb = await this._userManager.FindByIdAsync(userId);
            if (userFromDb == null)
            {
                return false;
            }

            await this._userManager.RemovePasswordAsync(userFromDb);
            await this._userManager.AddPasswordAsync(userFromDb, password);

            return true;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var userFromDb = await this._usersRepository.GetByIdWithDeletedAsync(userId);
            if (userFromDb == null) { return false; }
            this._usersRepository.Delete(userFromDb);
            await this._usersRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreUserAsync(string userId)
        {
            var userFromDb = await this._usersRepository.GetByIdWithDeletedAsync(userId);
            if (userFromDb == null) { return false; }
            this._usersRepository.Undelete(userFromDb);
            await this._usersRepository.SaveChangesAsync();
            return true;
        }

        public Task<bool> IsRegistered(string userName)
        {
            var userExists = this._usersRepository.AllAsNoTrackingWithDeleted().FirstOrDefault(u => u.UserName == userName);
            return Task.Run(() => userExists != null);
        }

        public async Task PopulateCommonUserModelPropertiesAsync(UserProfileInputModel userModel)
        {
            //TODO: Replace the partial view with a component and put this logic there

            var vendors = await this.GetAllInRoleAsync<UserProfileInputModel>(GlobalConstants.Roles.VendorRoleName);

            userModel.AvailableRoles = await this._roleManager.Roles.To<SelectListItem>().ToListAsync();
            userModel.AvailableRoles.ForEach(r => { r.Selected = r.Value == userModel.RoleName; });
            userModel.Company = userModel.Company ?? new UserProfileCompanyInputModel();
            userModel.Company.ContactAddress = userModel.Company.ContactAddress ?? new UserProfileAddressInputModel();
            userModel.Company.ShippingAddress = userModel.Company.ShippingAddress ?? new UserProfileAddressInputModel();
            userModel.ContactAddress = userModel.ContactAddress ?? new UserProfileAddressInputModel();

            if (vendors.Any())
            {
                userModel.AvailableRoles.RemoveAll(r => r.Text == GlobalConstants.Roles.VendorRoleName);
            }

            var assembly = Assembly.GetEntryAssembly();
            var resourceStream = assembly?.GetManifestResourceStream("Vxp.Web.Resources.AllCountriesInTheWorlds.csv");

            if (resourceStream != null)
            {
                var allCountries = new HashSet<SelectListItem>();
                using (var reader = new StreamReader(resourceStream))
                {
                    string country;
                    while ((country = reader.ReadLine()) != null)
                    {
                        allCountries.Add(new SelectListItem(country, country));
                    }
                }

                allCountries.Add(new SelectListItem("- Select Country -", string.Empty, true, false));
                userModel.AvailableCountries = allCountries;
            }

            if (!string.IsNullOrEmpty(userModel.UserId)) // new user
            {
                var user = await this._usersRepository.GetByIdWithDeletedAsync(userModel.UserId);
                userModel.IsEmailConfirmed = await this._userManager.IsEmailConfirmedAsync(user);
            }

        }

        private Task<IQueryable<TViewModel>> GetAllUsers<TViewModel>(bool includeDeleted,
            Expression<Func<ApplicationUser, bool>> exp = null)
        {
            var query = (includeDeleted ? this._usersRepository.AllWithDeleted() : this._usersRepository.All())
                .Include(user => user.Company)
                .Include(user => user.Roles)
                .ThenInclude(role => role.Role)
                .Include(user => user.BankAccounts)
                .Include(user => user.Distributors)
                .ThenInclude(distributorUser => distributorUser.DistributorKey)
                .ThenInclude(distributorKey => distributorKey.BankAccount)
                .ThenInclude(bankAccount => bankAccount.Owner)
                .ThenInclude(bankAccountOwner => bankAccountOwner.Company).AsQueryable();

            if (exp != null)
            {
                query = query.Where(exp);
            }

            return Task.Run(() => query.To<TViewModel>());
        }
    }
}
