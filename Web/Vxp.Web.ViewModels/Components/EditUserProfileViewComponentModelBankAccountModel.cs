using System.ComponentModel.DataAnnotations;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Components
{
    public class EditUserProfileViewComponentModelBankAccountModel : IMapFrom<BankAccount>
    {
        [Required]
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)] public string AccountNumber { get; set; }
        [Required(AllowEmptyStrings = false)] public string BicCode { get; set; }
        public string SwiftCode { get; set; }
        [Required(AllowEmptyStrings = false)] public string BankName { get; set; }
    }
}