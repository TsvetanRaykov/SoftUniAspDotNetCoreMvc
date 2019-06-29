using System;
using Vxp.Data.Models.Contracts;

namespace Vxp.Data.Models
{
    public abstract class BaseModel : IAuditInfo
    {
        protected BaseModel()
        {
            this.CreatedOn = DateTime.UtcNow;
        }
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }
}