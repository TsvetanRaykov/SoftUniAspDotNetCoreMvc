// ReSharper disable VirtualMemberCallInConstructor

namespace Vxp.Data.Models
{
    using System.Collections.Generic;

    using Vxp.Data.Common.Models;

    public class BankAccount : BaseModel<int>
    {
        public BankAccount()
        {
            this.DistributorKeys = new HashSet<DistributorKey>();
        }

        public string AccountNumber { get; set; }

        public string BicCode { get; set; }

        public string SwiftCode { get; set; }

        public string BankName { get; set; }

        public virtual ICollection<DistributorKey> DistributorKeys { get; set; }

        public virtual ApplicationUser Owner { get; set; }
    }
}