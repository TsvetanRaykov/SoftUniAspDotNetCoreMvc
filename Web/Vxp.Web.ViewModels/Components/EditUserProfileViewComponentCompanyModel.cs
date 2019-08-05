namespace Vxp.Web.ViewModels.Components
{
    using Common;
    using Data.Models;
    using Infrastructure.Attributes.Validation;
    using Services.Mapping;
    using System.ComponentModel.DataAnnotations;

    public class EditUserProfileViewComponentCompanyModel : IMapFrom<Company>, IMapTo<Company>
    {
        public string RoleName { get; set; }

        public EditUserProfileViewComponentCompanyModel()
        {
            this.ContactAddress = new EditUserProfileViewComponentAddressModel();
            this.ShippingAddress = new EditUserProfileViewComponentAddressModel();
        }

        [Display(Name = "Company name")]
        [StringLength(30)]
        [RequiredInSpecificRoles(compareRoleProperty: nameof(RoleName), GlobalConstants.Roles.VendorRoleName, GlobalConstants.Roles.DistributorRoleName,
            ErrorMessage = "The Company name is required.")]
        public string Name { get; set; }

        [RequiredInSpecificRoles(compareRoleProperty: nameof(RoleName), GlobalConstants.Roles.VendorRoleName, GlobalConstants.Roles.DistributorRoleName,
            ErrorMessage = "The Company BIN is required.")]
        [Display(Name = "Business number")]
        [StringLength(15)]
        public string BusinessNumber { get; set; }

        public EditUserProfileViewComponentAddressModel ContactAddress { get; set; }

        public EditUserProfileViewComponentAddressModel ShippingAddress { get; set; }

    }
}