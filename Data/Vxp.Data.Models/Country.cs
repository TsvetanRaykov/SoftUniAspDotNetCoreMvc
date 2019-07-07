namespace Vxp.Data.Models
{
    using Vxp.Data.Common.Models;

    public class Country : BaseModel<int>
    {
        public string Name { get; set; }

        public string Language { get; set; }
    }
}