namespace Vxp.Data.Models
{
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

        public virtual ICollection<Product> Products { get; set; }

    }
}