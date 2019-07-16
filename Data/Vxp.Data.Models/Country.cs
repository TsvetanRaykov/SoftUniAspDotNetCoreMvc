using System.ComponentModel.DataAnnotations;

namespace Vxp.Data.Models
{
    using Vxp.Data.Common.Models;

    public class Country : BaseDeletableModel<int>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Language { get; set; }
    }
}