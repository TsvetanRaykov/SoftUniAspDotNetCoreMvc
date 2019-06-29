namespace Vxp.Data.Models
{
    public class DistributorUser
    {
        public int DistributorKeyId { get; set; }
        public virtual DistributorKey DistributorKey { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}