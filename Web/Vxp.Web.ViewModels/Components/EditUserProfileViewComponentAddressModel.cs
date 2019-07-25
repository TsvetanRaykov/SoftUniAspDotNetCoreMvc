using System.ComponentModel.DataAnnotations;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Components
{
    public class EditUserProfileViewComponentAddressModel : IMapFrom<Address>
    {
        [Display(Name = "City")]
        [StringLength(50)]
        public string City { get; set; }

        [Display(Name = "Address Location")]
        [StringLength(100)]
        public string AddressLocation { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(15)]
        [Phone]
        [Display(Name = "Phone number")]
        public string Phone { get; set; }

        [Display(Name = "Country name")]
        [StringLength(30)]
        public string CountryName { get; set; }
    }
}