namespace Vxp.Services.Models
{
    using Vxp.Data.Models;
    using Mapping;

    public class ProjectDto : IMapTo<Project>, IMapFrom<Project>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerId { get; set; }
        public string PartnerId { get; set; }
    }
}