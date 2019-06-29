namespace Vxp.Data.Models
{
    public class ProductDetail : BaseModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual Product Product { get; set; }
    }
}