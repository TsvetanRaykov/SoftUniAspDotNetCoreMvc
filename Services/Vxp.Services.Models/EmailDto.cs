using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Services.Models
{
    public class EmailDto : IMapTo<CustomerInvitation>
    {
        public string EmailTo { get; set; }

        public string Subject { get; set; }

        public string MessageBody { get; set; }

        public string DistributorKey { get; set; }

    }
}