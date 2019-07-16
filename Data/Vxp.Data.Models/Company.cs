// ReSharper disable VirtualMemberCallInConstructor

using System.ComponentModel.DataAnnotations;

namespace Vxp.Data.Models
{
    using System.Collections.Generic;

    using Vxp.Data.Common.Models;

    public class Company : BaseDeletableModel<int>
    {
        public Company()
        {
            this.Members = new HashSet<ApplicationUser>();
        }

        [Required]
        public string Name { get; set; }

        public string BusinessNumber { get; set; }

        public virtual Address ContactAddress { get; set; }

        public virtual Address ShippingAddress { get; set; }

        public virtual ICollection<ApplicationUser> Members { get; set; }
    }
}