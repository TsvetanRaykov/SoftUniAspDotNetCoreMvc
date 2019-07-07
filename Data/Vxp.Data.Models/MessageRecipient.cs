namespace Vxp.Data.Models
{
    using System;

    using Vxp.Data.Common.Models;

    public class MessageRecipient : BaseModel<int>
    {
        public int MessageId { get; set; }

        public virtual Message Message { get; set; }

        public string RecipientId { get; set; }

        public virtual ApplicationUser Recipient { get; set; }

        public DateTime? ReadOn { get; set; }
    }
}