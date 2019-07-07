namespace Vxp.Data.Models
{
    using Vxp.Data.Common.Models;

    public class OrderProduct : BaseModel<int>
    {
        public int Quantity { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}