using System.ComponentModel.DataAnnotations;

namespace Vxp.Web.ViewModels.Administration.Dashboard
{
    public class UserIdInputModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(36)]
        public string Id { get; set; }
    }
}