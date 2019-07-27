using System.ComponentModel.DataAnnotations;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Components
{
    public class EditUserProfileViewComponentCompanyModel : IMapFrom<Company>, IMapTo<Company>
    {
        public EditUserProfileViewComponentCompanyModel()
        {
            this.ContactAddress = new EditUserProfileViewComponentAddressModel();
            this.ShippingAddress = new EditUserProfileViewComponentAddressModel();
        }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Company name")]
        [StringLength(30)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Business number")]
        [StringLength(15)]
        public string BusinessNumber { get; set; }

        public EditUserProfileViewComponentAddressModel ContactAddress { get; set; }

        public EditUserProfileViewComponentAddressModel ShippingAddress { get; set; }

    }
}