namespace Vxp.Web.ViewModels.Customer.Distributors
{
    using System.Collections.Generic;

    public class DistributorsListViewModel
    {
        public DistributorsListViewModel()
        {
            this.Distributors = new List<DistributorListViewModel>();
            this.DistributorKeyRegisterInputModel = new DistributorKeyRegisterInputModel();
        }
        public List<DistributorListViewModel> Distributors { get; set; }

        public DistributorKeyRegisterInputModel DistributorKeyRegisterInputModel { get; set; }
    }
}