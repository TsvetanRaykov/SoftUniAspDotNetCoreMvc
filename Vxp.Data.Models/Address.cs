namespace Vxp.Data.Models
{
    public class Address : BaseModel
    {
        public string City { get; set; }
        public string AddressLocation { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public virtual Country Country { get; set; }
    }
}