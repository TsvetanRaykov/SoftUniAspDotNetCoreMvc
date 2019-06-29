namespace Vxp.Data.Models
{
    public class OrderProduct : BaseModel
    {
        public int Quantity { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}