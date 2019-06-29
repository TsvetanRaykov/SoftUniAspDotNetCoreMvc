using System.Collections.Generic;

namespace Vxp.Data.Models
{
    public class BankAccount : BaseModel
    {
        public BankAccount()
        {
            this.DistributorKeys = new HashSet<DistributorKey>();
        }

        public string AccounNumber { get; set; }
        public string BicCode { get; set; }
        public string SwiftCode { get; set; }
        public string BankName { get; set; }
        public virtual ICollection<DistributorKey> DistributorKeys { get; set; }
        public virtual ApplicationUser Owner { get; set; }
    }
}