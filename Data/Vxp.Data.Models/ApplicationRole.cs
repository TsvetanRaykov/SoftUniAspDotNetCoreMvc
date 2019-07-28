// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace Vxp.Data.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;
    using System;
    using Vxp.Data.Common.Models;

    public class ApplicationRole : IdentityRole, IAuditInfo, IDeletableEntity
    {
        public ApplicationRole()
            : this(null)
        {
        }

        public ApplicationRole(string name)
            : base(name)
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public virtual ICollection<ApplicationUserRole<string>> Users { get; set; } = new List<ApplicationUserRole<string>>();

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
