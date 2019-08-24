namespace Vxp.Services.Data.Products
{
    using System.Linq;
    using System.Threading.Tasks;
    using Vxp.Data.Common.Repositories;
    using Vxp.Data.Models;
    using Mapping;

    public class ProductPricesService : IProductPricesService
    {
        private readonly IDeletableEntityRepository<PriceModifier> _priceModifiersRepository;

        public ProductPricesService(IDeletableEntityRepository<PriceModifier> priceModifiersRepository)
        {
            this._priceModifiersRepository = priceModifiersRepository;
        }

        public IQueryable<TViewModel> GetBuyerPriceModifiers<TViewModel>(string userName)
        {
            var modifiers = this._priceModifiersRepository.AllAsNoTracking()
                .Where(m => m.Buyer.UserName == userName).To<TViewModel>();

            return modifiers;
        }

        public Task<bool> DeleteProductPriceModifierAsync(int priceModifier)
        {
            throw new System.NotImplementedException();
        }

        public async Task SetProductPriceModifierAsync<TViewModel>(TViewModel priceModifier)
        {
            var modifier = AutoMapper.Mapper.Map<PriceModifier>(priceModifier);

            var priceModifierFromDb = await this._priceModifiersRepository.GetByIdWithDeletedAsync(modifier.Id);

            if (priceModifierFromDb == null)
            {
                await this._priceModifiersRepository.AddAsync(modifier);
            }
            else
            {
                AutoMapper.Mapper.Map(priceModifier, priceModifierFromDb);
            }

            await this._priceModifiersRepository.SaveChangesAsync();

        }
    }
}