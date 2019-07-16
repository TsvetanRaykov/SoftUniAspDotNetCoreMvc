// ReSharper disable VirtualMemberCallInConstructor

namespace Vxp.Data.Models
{
    using System.Collections.Generic;

    using Vxp.Data.Common.Models;

    public class Message : BaseDeletableModel<int>
    {
        public Message()
        {
            this.Recipients = new HashSet<MessageRecipient>();
        }

        public string Body { get; set; }

        public virtual Message Topic { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public virtual ICollection<MessageRecipient> Recipients { get; set; }
    }
}