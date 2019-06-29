using System;

namespace Vxp.Data.Models.Contracts
{
    public interface IAuditInfo
    {
        DateTime CreatedOn { get; set; }
        DateTime? ModifiedOn { get; set; }
    }
}