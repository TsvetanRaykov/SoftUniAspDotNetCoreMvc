using System.Collections.Generic;

namespace Vxp.Data.Models
{
    public class ProductCategory : BaseModel
    {
        public ProductCategory()
        {
            this.Products = new HashSet<Product>();
        }
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }

    }
}