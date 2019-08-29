namespace Vxp.Data.Models
{
    using Common.Enums;
    using Vxp.Data.Common.Models;

    public class OrderHistory : BaseDeletableModel<int>
    {
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public OrderStatus OldStatus { get; set; }

        public OrderStatus NewStatus { get; set; }
    }
}