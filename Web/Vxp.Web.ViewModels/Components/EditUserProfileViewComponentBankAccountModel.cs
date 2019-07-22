using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Components
{
    public class EditUserProfileViewComponentBankAccountModel : IMapFrom<BankAccount>
    {
        public string AccountNumber { get; set; }
        public string BicCode { get; set; }
        public string SwiftCode { get; set; }
        public string BankName { get; set; }
    }
}