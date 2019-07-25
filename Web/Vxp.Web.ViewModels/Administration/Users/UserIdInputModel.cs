using System.ComponentModel.DataAnnotations;

namespace Vxp.Web.ViewModels.Administration.Users
{
    public class UserIdInputModel
    {
        [Required]
        public string Id { get; set; }
    }
}