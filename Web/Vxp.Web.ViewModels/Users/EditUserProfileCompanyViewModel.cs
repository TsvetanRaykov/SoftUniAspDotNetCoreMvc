using System.ComponentModel.DataAnnotations;
using Vxp.Web.ViewModels.ModelBinders;

namespace Vxp.Web.ViewModels.Users
{
    using Microsoft.AspNetCore.Mvc;
    using Common;
    using Data.Models;
    using Services.Mapping;
    using Infrastructure.Attributes.Validation;

    public class EditUserProfileCompanyViewModel : IMapFrom<Company>, IMapTo<Company>
    {
        [ModelBinder(typeof(TransparentPropertyModelBinder))]
        public string RoleName { get; set; }

        public EditUserProfileCompanyViewModel()
        {
            this.ContactAddress = new EditUserProfileAddressViewModel();
            this.ShippingAddress = new EditUserProfileAddressViewModel();
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

        public EditUserProfileAddressViewModel ContactAddress { get; set; }

        public EditUserProfileAddressViewModel ShippingAddress { get; set; }

    }
}