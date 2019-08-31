namespace Vxp.Web.ViewModels.Customer.Distributors
{
    using System.Collections.Generic;
    using Projects;

    public class DistributorViewModel
    {
        public DistributorViewModel()
        {
            this.Details = new DistributorViewModelDetails();
            this.Messages = new List<string>();
            this.ExistingProjects = new List<ProjectInputModel>();
        }

        public string Id { get; set; }
        public DistributorViewModelDetails Details { get; set; }
        public IEnumerable<string> Messages { get; set; }

        public List<ProjectInputModel> ExistingProjects { get; set; }
    }
}