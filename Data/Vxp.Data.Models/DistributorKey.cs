// ReSharper disable VirtualMemberCallInConstructor
namespace Vxp.Data.Models
{
    using System;
    using System.Collections.Generic;

    using Vxp.Data.Common.Models;

    public class DistributorKey : BaseModel<int>
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