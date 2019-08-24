namespace Vxp.Web.ViewModels.Customer.Distributors
{
    using System.Collections.Generic;

    public class DistributorViewModel
    {
        public DistributorViewModel()
        {
            this.Details = new DistributorViewModelDetails();
            this.Messages = new List<string>();
        }
        public DistributorViewModelDetails Details { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }
}