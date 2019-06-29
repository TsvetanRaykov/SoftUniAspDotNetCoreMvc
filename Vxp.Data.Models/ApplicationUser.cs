﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Vxp.Data.Models.Contracts;

namespace Vxp.Data.Models
{
    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {

        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
            this.BankAccounts = new HashSet<BankAccount>();
            this.Distributors = new HashSet<DistributorUser>();
            this.Projects = new HashSet<Project>();
            this.ReceivedMessages = new HashSet<MessageRecipient>();
            this.PriceModifiersReceive = new HashSet<PriceModifier>();
            this.PriceModifiersGive = new HashSet<PriceModifier>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual Address ContactAddress { get; set; }
        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public virtual ICollection<BankAccount> BankAccounts { get; set; }

        public virtual ICollection<DistributorUser> Distributors { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<MessageRecipient> ReceivedMessages { get; set; }
        public virtual ICollection<PriceModifier> PriceModifiersReceive { get; set; }
        public virtual ICollection<PriceModifier> PriceModifiersGive { get; set; }
    }
}