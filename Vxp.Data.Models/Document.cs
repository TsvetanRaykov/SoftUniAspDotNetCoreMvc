using System;
using Vxp.Data.Models.Enums;

namespace Vxp.Data.Models
{
    public class Document : BaseModel
    {

        public DocumentType Type { get; set; }

        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime DocumentDate { get; set; }

        public virtual Project Project { get; set; }

    }
}