namespace Vxp.Web.ViewModels.Projects
{
    using Data.Models;
    using Services.Mapping;

    public class ProjectViewModel : IMapFrom<Project>
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}