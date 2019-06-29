using System.Collections.Generic;

namespace Vxp.Data.Models
{
    public class Project : BaseModel
    {
        public Project()
        {
            this.Documents = new HashSet<Document>();
            this.Orders = new HashSet<Order>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<Document> Documents { get; set; }

        public virtual ApplicationUser Owner { get; set; }

    }
}