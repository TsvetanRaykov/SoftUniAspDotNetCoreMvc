using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vxp.Services.Data.Users;
using Vxp.Web.ViewModels.Users;

namespace Vxp.Web.Areas.Vendor.Controllers
{
    public class DistributorsController : VendorsController
    {
        private readonly IUsersService _usersService;

        public DistributorsController(IUsersService usersService)
        {
            this._usersService = usersService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public async Task<IActionResult> Register()
        {
            var viewModel = new UserProfileViewModel
            {
                IsNewUser = true
            };
            await this._usersService.PopulateCommonUserModelProperties(viewModel);

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserProfileViewModel inputModel)
        {

            return this.View(inputModel);
        }
    }
}