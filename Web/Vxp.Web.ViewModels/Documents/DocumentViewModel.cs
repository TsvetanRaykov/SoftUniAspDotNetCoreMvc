using System;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Documents
{
    public class DocumentViewModel : IMapFrom<Document>
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string OriginalFileName { get; set; }
        public DateTime DocumentDate { get; set; }
    }
}