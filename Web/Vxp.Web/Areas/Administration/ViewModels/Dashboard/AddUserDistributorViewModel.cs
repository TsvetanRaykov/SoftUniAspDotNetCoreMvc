using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.Areas.Administration.ViewModels.Dashboard
{
    public class AddUserDistributorViewModel : IMapFrom<ApplicationUser>
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
        public string CompanyName { get; set; }

        public string DisplayName => $"{this.FirstName} {this.LastName} [{this.Email}] {this.CompanyName}";
    }
}