namespace Vxp.Data.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Vxp.Data.Common.Models;

    public class ProductCategory : BaseDeletableModel<int>
    {
        public ProductCategory()
        {
            this.Products = new HashSet<Product>();
        }

        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }

    }
}