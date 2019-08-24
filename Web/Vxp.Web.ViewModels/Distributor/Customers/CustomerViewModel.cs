namespace Vxp.Web.ViewModels.Distributor.Customers
{
    using System.Collections.Generic;
    using Prices;

    public class CustomerViewModel
    {
        public CustomerViewModel()
        {
            this.Details = new CustomerViewModelDetails();
            this.Messages = new List<string>();
        }
        public CustomerViewModelDetails Details { get; set; }
        public IEnumerable<string> Messages { get; set; }
        public PriceModifierInputModel PriceModifierInputModel { get; set; }

    }
}