using System.ComponentModel.DataAnnotations;
using Vxp.Web.ViewModels.ModelBinders;

namespace Vxp.Web.ViewModels.Users
{
    using Microsoft.AspNetCore.Mvc;
    using Common;
    using Data.Models;
    using Services.Mapping;
    using Infrastructure.Attributes.Validation;

    public class UserProfileCompanyViewModel : IMapFrom<Company>, IMapTo<Company>
    {
        [ModelBinder(typeof(TransparentPropertyModelBinder))]
        public string RoleName { get; set; }

        public UserProfileCompanyViewModel()
        {
            this.ContactAddress = new UserProfileAddressViewModel();
            this.ShippingAddress = new UserProfileAddressViewModel();
        }

        [Display(Name = "Company name")]
        [StringLength(30)]
        [RequiredInSpecificRoles(compareRoleProperty: nameof(RoleName), GlobalConstants.Roles.VendorRoleName, GlobalConstants.Roles.DistributorRoleName,
            ErrorMessage = "The Company name is required.")]
        [Remote(action: "VerifyCompanyName", controller: "Users", AdditionalFields = nameof(BusinessNumber), HttpMethod = "Post")]
        public string Name { get; set; }

        [Display(Name = "Business number")]
        [StringLength(15)]
        [RequiredInSpecificRoles(compareRoleProperty: nameof(RoleName), GlobalConstants.Roles.VendorRoleName, GlobalConstants.Roles.DistributorRoleName,
            ErrorMessage = "The Company BIN is required.")]
        [Remote(action: "VerifyBusinessNumber", controller: "Users", AdditionalFields = nameof(Name), HttpMethod = "Post")]
        public string BusinessNumber { get; set; }

        public UserProfileAddressViewModel ContactAddress { get; set; }

        public UserProfileAddressViewModel ShippingAddress { get; set; }

    }
}