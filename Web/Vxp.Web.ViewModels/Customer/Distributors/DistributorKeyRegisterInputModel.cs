using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Vxp.Web.ViewModels.Customer.Distributors
{
    public class DistributorKeyRegisterInputModel
    {
        [Display(Name = "Distributor Key")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(36, MinimumLength = 36)]
        [Remote(action: "ValidateDistributorKey", controller: "Distributors", HttpMethod = "Post")]
        public string DistributorKey { get; set; }
    }
}