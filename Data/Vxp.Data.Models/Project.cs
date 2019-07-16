// ReSharper disable VirtualMemberCallInConstructor

namespace Vxp.Data.Models
{
    using System.Collections.Generic;

    using Vxp.Data.Common.Models;

    public class Project : BaseDeletableModel<int>
    {
        public Project()
        {
            this.Documents = new HashSet<Document>();
            this.Orders = new HashSet<Order>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<Document> Documents { get; set; }

        public virtual ApplicationUser Owner { get; set; }

    }
}