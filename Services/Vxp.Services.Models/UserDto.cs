namespace Vxp.Services.Models
{
    using Vxp.Data.Models;
    using Mapping;

    public class UserDto : IMapFrom<ApplicationUser>, IMapTo<ApplicationUser>
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}