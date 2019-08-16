namespace Vxp.Data.Models
{
    using Vxp.Data.Common.Models;

    public class ProductDetail : BaseModel<int>
    {
        public string Value { get; set; }

        public virtual Product Product { get; set; }

        public virtual CommonProductDetail CommonDetail { get; set; }
    }
}