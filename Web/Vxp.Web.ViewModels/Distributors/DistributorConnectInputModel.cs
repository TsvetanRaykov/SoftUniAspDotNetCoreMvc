using System.ComponentModel.DataAnnotations;

namespace Vxp.Web.ViewModels.Distributors
{
    public class DistributorConnectInputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Customer email")]
        public string CustomerEmail { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Distributor email")]
        public string DistributorEmail { get; set; }

        [Required]
        [StringLength(36)]
        public string CustomerId { get; set; }
    }
}
