// ReSharper disable VirtualMemberCallInConstructor

namespace Vxp.Data.Models
{

    using Vxp.Data.Common.Models;

    public class Message : BaseDeletableModel<int>
    {

        public string Body { get; set; }
        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public string RecipientId { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
    }
}