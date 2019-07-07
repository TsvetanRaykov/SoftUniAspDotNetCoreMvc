namespace Vxp.Data.Models
{
    using Vxp.Data.Common.Models;

    public class Address : BaseModel<int>
    {
        public string City { get; set; }

        public string AddressLocation { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public virtual Country Country { get; set; }
    }
}