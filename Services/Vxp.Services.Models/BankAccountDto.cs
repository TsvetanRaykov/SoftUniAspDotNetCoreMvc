namespace Vxp.Services.Models
{
    using Vxp.Data.Models;
    using Mapping;

    public class BankAccountDto : IMapFrom<BankAccount>
    {
        public int Id { get; set; }
    }
}