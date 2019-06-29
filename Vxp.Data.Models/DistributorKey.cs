using System;
using System.Collections.Generic;

namespace Vxp.Data.Models
{
    public class DistributorKey : BaseModel
    {
        public DistributorKey()
        {
            this.Customers = new HashSet<DistributorUser>();
        }

        public Guid KeyCode { get; set; }

        public virtual ICollection<DistributorUser> Customers { get; set; }

        public virtual BankAccount BankAccount { get; set; }
    }
}