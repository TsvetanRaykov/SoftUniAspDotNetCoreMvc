namespace Vxp.Services.Models
{
    using Vxp.Data.Models;
    using Mapping;

    public class BankAccountDto : IMapFrom<BankAccount>, IMapTo<BankAccount>
    {
        public int Id { get; set; }

        public string AccountNumber { get; set; }
    }
}