namespace Vxp.Data.Models
{
    using System;
    using Common.Enums;
    using Vxp.Data.Common.Models;

    public class Document : BaseDeletableModel<int>
    {

        public DocumentType Type { get; set; }

        public string OriginalFileName { get; set; }

        public string ContentType { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public DateTime DocumentDate { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

    }
}