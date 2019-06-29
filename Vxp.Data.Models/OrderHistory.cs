using Vxp.Data.Models.Enums;

namespace Vxp.Data.Models
{
    public class OrderHistory : BaseModel
    {
        public Order Order { get; set; }
        public OrderStatus OldStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
    }
}