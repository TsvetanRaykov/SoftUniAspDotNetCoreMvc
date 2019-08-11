﻿using System.Collections.Generic;

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

    public class UsersService : IUsersService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDeletableEntityRepository<ApplicationUser> _usersRepository;
        private readonly IDistributorsService _distributorsService;

        public UsersService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            IDistributorsService distributorsService)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
            this._usersRepository = usersRepository;
            this._distributorsService = distributorsService;
        }

        public Task<IQueryable<TViewModel>> GetAll<TViewModel>(Expression<Func<ApplicationUser, bool>> exp)
        {
            var query = this._usersRepository.AllWithDeleted()
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
    }
}
