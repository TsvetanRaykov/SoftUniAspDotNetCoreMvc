// ReSharper disable VirtualMemberCallInConstructor

using System.ComponentModel.DataAnnotations;

namespace Vxp.Data.Models
{
    using System;
    using System.Collections.Generic;

    using Vxp.Data.Common.Models;

    public class DistributorKey : BaseDeletableModel<int>
    {
        public DistributorKey()
        {
            this.Customers = new HashSet<DistributorUser>();
        }

        [Required]
        public Guid KeyCode { get; set; }

        public virtual ICollection<DistributorUser> Customers { get; set; }

        public virtual BankAccount BankAccount { get; set; }
    }
}