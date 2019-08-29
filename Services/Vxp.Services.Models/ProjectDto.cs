namespace Vxp.Services.Models
{
    using Vxp.Data.Models;
    using Mapping;

    public class ProjectDto : IMapTo<Project>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerId { get; set; }
    }
}