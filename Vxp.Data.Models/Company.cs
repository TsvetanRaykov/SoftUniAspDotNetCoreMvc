using System.Collections.Generic;

namespace Vxp.Data.Models
{
    public class Company : BaseModel
    {
        public Company()
        {
            this.Members = new HashSet<ApplicationUser>();
        }
        public string Name { get; set; }
        public string BusinessNumber { get; set; }

        public virtual Address ContactAddress { get; set; }

        public virtual Address ShippingAddress { get; set; }

        public virtual ICollection<ApplicationUser> Members { get; set; }
    }
}