namespace Vxp.Web.ViewModels.Projects
{
    using System.Collections.Generic;

    public class ProjectsListViewModel
    {
        public ProjectsListViewModel()
        {
            this.ExistingProjects = new List<ProjectInputModel>();
        }
        public ProjectInputModel Input { get; set; }
        public IEnumerable<ProjectInputModel> ExistingProjects { get; set; }


    }
}