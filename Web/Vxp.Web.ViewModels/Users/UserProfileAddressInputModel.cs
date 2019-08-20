namespace Vxp.Web.ViewModels.Users
{
    using Common;
    using Data.Models;
    using Infrastructure.Attributes.Validation;
    using Microsoft.AspNetCore.Mvc;
    using ModelBinders;
    using Services.Mapping;
    using System.ComponentModel.DataAnnotations;

    public class UserProfileAddressInputModel : IMapFrom<Address>, IMapTo<Address>
    {
        private const string RequiredGroupId = "Grp01";

        [ModelBinder(typeof(TransparentPropertyModelBinder))]
        public string RoleName { get; set; }

        [Display(Name = "City")]
        [StringLength(50)]
        [RequiredAsAGroup(RequiredGroupId,
            ErrorMessage = GlobalConstants.ErrorMessages.RequiredField)]
        public string City { get; set; }

        [Display(Name = "Address Location")]
        [StringLength(100)]
        [RequiredAsAGroup(RequiredGroupId,
            ErrorMessage = GlobalConstants.ErrorMessages.RequiredField)]
        public string AddressLocation { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
            ErrorMessage = GlobalConstants.ErrorMessages.EmailInvalid)]
        [RequiredAsAGroup(RequiredGroupId,
            ErrorMessage = GlobalConstants.ErrorMessages.RequiredField)]
        public string Email { get; set; }

        [RegularExpression(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$",
            ErrorMessage = GlobalConstants.ErrorMessages.PhoneInvalid)]
        [Display(Name = "Phone number")]
        [RequiredAsAGroup(RequiredGroupId,
            ErrorMessage = GlobalConstants.ErrorMessages.RequiredField)]
        public string Phone { get; set; }

        [Display(Name = "Country name")]
        [StringLength(30)]
        [RequiredAsAGroup(RequiredGroupId,
            ErrorMessage = GlobalConstants.ErrorMessages.RequiredField)]
        public string CountryName { get; set; }
    }
   
}