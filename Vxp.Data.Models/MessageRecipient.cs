using System;

namespace Vxp.Data.Models
{
    public class MessageRecipient : BaseModel
    {
        public int MessageId { get; set; }
        public virtual Message Message { get; set; }

        public string RecipientId { get; set; }
        public virtual ApplicationUser Recipient { get; set; }

        public DateTime? ReadOn { get; set; }
    }
}