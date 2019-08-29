using Newtonsoft.Json;

namespace Vxp.Data.Models
{
    using System.Collections.Generic;
    using Vxp.Data.Common.Models;
    public class CommonProductDetail : BaseDeletableModel<int>
    {
        public CommonProductDetail()
        {
            this.ProductDetails = new HashSet<ProductDetail>();
        }
        public string Name { get; set; }

        public string Measure { get; set; }

        [JsonIgnore]
        public ICollection<ProductDetail> ProductDetails { get; set; }

    }
}
