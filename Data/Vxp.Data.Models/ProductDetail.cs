namespace Vxp.Data.Models
{
    using Vxp.Data.Common.Models;

    public class ProductDetail : BaseModel<int>
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public virtual Product Product { get; set; }
    }
}