using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Components
{
    public class EditUserProfileViewComponentAddressModel : IMapFrom<Address>
    {
        public string City { get; set; }
        public string AddressLocation { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CountryName { get; set; }
    }
}