﻿namespace Vxp.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Vxp.Data.Common.Models;

    public class BankAccount : BaseDeletableModel<int>
    {
        public BankAccount()
        {
            this.DistributorKeys = new HashSet<DistributorKey>();
        }

        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string BicCode { get; set; }

        public string SwiftCode { get; set; }
        [Required]
        public string BankName { get; set; }

        public virtual ICollection<DistributorKey> DistributorKeys { get; set; }

        [Required]
        public string OwnerId { get; set; }

        public virtual ApplicationUser Owner { get; set; }
    }
}