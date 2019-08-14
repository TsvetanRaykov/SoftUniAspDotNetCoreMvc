using System.IO;
using System.Reflection;
using Vxp.Common;
using Vxp.Web.ViewModels.Administration.Users;
using Vxp.Web.ViewModels.Users;

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

    public class UsersService : IUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IDeletableEntityRepository<ApplicationUser> _usersRepository;

        public UsersService(
            UserManager<ApplicationUser> userManager,
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            RoleManager<ApplicationRole> roleManager)
        {
            this._userManager = userManager;
            this._usersRepository = usersRepository;
            this._roleManager = roleManager;
        }

        public Task<IQueryable<TViewModel>> GetAll<TViewModel>(Expression<Func<ApplicationUser, bool>> exp)
        {
            return GetAllUsers<TViewModel>(false, exp);
        }

        public Task<IQueryable<TViewModel>> GetAllWithDeleted<TViewModel>(Expression<Func<ApplicationUser, bool>> exp = null)
        {
            return GetAllUsers<TViewModel>(true, exp);
        }

        public Task<IQueryable<TViewModel>> GetAllInRoleAsync<TViewModel>(string roleName)
        {
            var applicationUsersInRole = this._usersRepository.AllAsNoTracking()
                .Where(u => u.Roles.Any(r => r.Role.Name == roleName));

            return Task.Run(() => applicationUsersInRole.To<TViewModel>());
        }

        public async Task<string> CreateUser<TViewModel>(TViewModel userModel, string password, string role)
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

            applicationUser.Email = applicationUser.ContactAddress.Email;
            var user = await this._userManager.CreateAsync(applicationUser, password);

            if (!user.Succeeded) { return null; }

            await this._userManager.AddToRoleAsync(applicationUser, role);

            return applicationUser.Id;

        }

        public async Task<bool> UpdateUser<TViewModel>(TViewModel userModel, IEnumerable<string> roleNames)
        {
            var applicationUser = AutoMapper.Mapper.Map<ApplicationUser>(userModel);

            var userFromDb = this._usersRepository.AllWithDeleted().FirstOrDefault(u => u.Id == applicationUser.Id);

            if (userFromDb == null)
            {
                return false;
            }

            AutoMapper.Mapper.Map(userModel, userFromDb);

            if (userFromDb.Company.Name == null)
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

        public async Task<bool> DeleteUser(string userId)
        {
            var userFromDb = await this._usersRepository.GetByIdWithDeletedAsync(userId);
            if (userFromDb == null) { return false; }
            this._usersRepository.Delete(userFromDb);
            await this._usersRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreUser(string userId)
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

        public async Task PopulateCommonUserModelProperties(UserProfileViewModel userModel)
        {
            //TODO: Replace the partial view with a component and put this logic there

            var vendors = await this.GetAllInRoleAsync<AddUserDistributorViewModel>(GlobalConstants.Roles.VendorRoleName);

            userModel.AvailableRoles = await this._roleManager.Roles.To<SelectListItem>().ToListAsync();
            userModel.AvailableRoles.ForEach(r => { r.Selected = r.Value == userModel.RoleName; });
            userModel.Company = userModel.Company ?? new UserProfileCompanyViewModel();
            userModel.Company.ContactAddress = userModel.Company.ContactAddress ?? new UserProfileAddressViewModel();
            userModel.Company.ShippingAddress = userModel.Company.ShippingAddress ?? new UserProfileAddressViewModel();
            userModel.ContactAddress = userModel.ContactAddress ?? new UserProfileAddressViewModel();

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

                allCountries.Add(new SelectListItem("- Select Country -", string.Empty, true, true));
                userModel.AvailableCountries = allCountries;
            }

            var user = await this._userManager.FindByIdAsync(userModel.UserId);
            userModel.IsEmailConfirmed = await this._userManager.IsEmailConfirmedAsync(user);
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
