using System.Collections.Generic;

namespace Vxp.Data.Models
{
    public class Message : BaseModel
    {
        public Message()
        {
            this.Recipients = new HashSet<MessageRecipient>();
        }

        public string Body { get; set; }

        public virtual Message Topic { get; set; }

        public ApplicationUser Author { get; set; }

        public virtual ICollection<MessageRecipient> Recipients { get; set; }
    }
}