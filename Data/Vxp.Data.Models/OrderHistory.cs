namespace Vxp.Data.Models
{
    using Vxp.Data.Common.Enums;
    using Vxp.Data.Common.Models;

    public class OrderHistory : BaseModel<int>
    {
        public Order Order { get; set; }

        public OrderStatus OldStatus { get; set; }

        public OrderStatus NewStatus { get; set; }
    }
}