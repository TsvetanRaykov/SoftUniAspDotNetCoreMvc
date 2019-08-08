using System.ComponentModel.DataAnnotations;
using Vxp.Data.Models;
using Vxp.Services.Mapping;
using Vxp.Web.Infrastructure.Attributes.Validation;

namespace Vxp.Web.ViewModels.Users
{
    public class EditUserProfileAddressViewModel : IMapFrom<Address>, IMapTo<Address>
    {
        private const string RequiredAsAGroupErrorMessage = "{0} is required.";
        public string RoleName { get; set; }

        [Display(Name = "City")]
        [StringLength(50)]
        [RequiredAsAGroup("Grp01", ErrorMessage = RequiredAsAGroupErrorMessage)]
        public string City { get; set; }

        [Display(Name = "Address Location")]
        [StringLength(100)]
        [RequiredAsAGroup("Grp01")]
        public string AddressLocation { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
            ErrorMessage = "The {0} field is not a valid e-mail address.")]
        [RequiredAsAGroup("Grp01")]
        public string Email { get; set; }

        [RegularExpression(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$", ErrorMessage = "The {0} field is not a valid phone.")]
        [Display(Name = "Phone number")]
        [RequiredAsAGroup("Grp01")]
        public string Phone { get; set; }

        [Display(Name = "Country name")]
        [StringLength(30)]
        [RequiredAsAGroup("Grp01")]
        public string CountryName { get; set; }
    }
}