using System.ComponentModel.DataAnnotations;
using Vxp.Web.ViewModels.ModelBinders;

namespace Vxp.Web.ViewModels.Users
{
    using Microsoft.AspNetCore.Mvc;
    using Common;
    using Data.Models;
    using Services.Mapping;
    using Infrastructure.Attributes.Validation;

    public class UserProfileCompanyInputModel : IMapFrom<Company>, IMapTo<Company>
    {

        public UserProfileCompanyInputModel()
        {
            this.ContactAddress = new UserProfileAddressInputModel();
            this.ShippingAddress = new UserProfileAddressInputModel();
        }

        [ModelBinder(typeof(TransparentPropertyModelBinder))]
        public string RoleName { get; set; }

        [Display(Name = "Company name")]
        [StringLength(30)]
        [RequiredInSpecificRoles(compareRoleProperty: nameof(RoleName), GlobalConstants.Roles.VendorRoleName, GlobalConstants.Roles.DistributorRoleName,
            ErrorMessage = "The Company name is required.")]
        [RequiredAsAGroup("Grp02", ErrorMessage = GlobalConstants.ErrorMessages.RequiredField)]
        public string Name { get; set; }

        [Display(Name = "Business number")]
        [StringLength(15)]
        [RequiredInSpecificRoles(compareRoleProperty: nameof(RoleName), GlobalConstants.Roles.VendorRoleName, GlobalConstants.Roles.DistributorRoleName,
            ErrorMessage = "The Company BIN is required.")]
        [RequiredAsAGroup("Grp02", ErrorMessage = GlobalConstants.ErrorMessages.RequiredField)]
        public string BusinessNumber { get; set; }

        public UserProfileAddressInputModel ContactAddress { get; set; }

        public UserProfileAddressInputModel ShippingAddress { get; set; }

    }
}