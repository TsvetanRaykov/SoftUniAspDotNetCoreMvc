namespace Vxp.Data.Models
{
    public class ProductImage : BaseModel
    {
        public string Url { get; set; }
        public string Alt { get; set; }
        public string Title { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}