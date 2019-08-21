using System;
using Vxp.Data.Common.Models;

namespace Vxp.Data.Models
{
    public class CustomerInvitation : BaseDeletableModel<int>
    {
        public string MessageBody { get; set; }
        public string Subject { get; set; }
        public string EmailTo { get; set; }
        public string DistributorKey { get; set; }
        public DateTime? TimeAccepted { get; set; }
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
    }
}