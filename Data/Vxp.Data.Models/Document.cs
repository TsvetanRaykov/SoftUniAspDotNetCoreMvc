namespace Vxp.Data.Models
{
    using System;

    using Vxp.Data.Common.Enums;
    using Vxp.Data.Common.Models;

    public class Document : BaseDeletableModel<int>
    {

        public DocumentType Type { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public DateTime DocumentDate { get; set; }

        public virtual Project Project { get; set; }

    }
}