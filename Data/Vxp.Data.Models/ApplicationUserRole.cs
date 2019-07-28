namespace Vxp.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System;

    public class ApplicationUserRole<TKey> : IdentityUserRole<TKey> where TKey : IEquatable<TKey>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
}