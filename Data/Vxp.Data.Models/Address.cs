using System.ComponentModel.DataAnnotations;

namespace Vxp.Data.Models
{
    using Vxp.Data.Common.Models;

    public class Address : BaseDeletableModel<int>
    {
        [Required]
        public string City { get; set; }
        [Required]
        public string AddressLocation { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }

    }
}