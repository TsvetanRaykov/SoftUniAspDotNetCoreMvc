namespace Vxp.Services.Data.Products
{
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using Vxp.Data.Common.Repositories;
    using Vxp.Data.Models;

    public class ProductDetailsService : IProductDetailsService
    {
        private readonly IDeletableEntityRepository<CommonProductDetail> _commonProductDetailsRepository;

        public ProductDetailsService(IDeletableEntityRepository<CommonProductDetail> commonProductDetailsRepository)
        {
            this._commonProductDetailsRepository = commonProductDetailsRepository;
        }

        public async Task<TViewModel> CreateCommonProductDetailAsync<TViewModel>(TViewModel commonProductDetail)
        {
            var newCommonProductProperty = AutoMapper.Mapper.Map<CommonProductDetail>(commonProductDetail);

            var deletedCommonProperty = this._commonProductDetailsRepository.AllWithDeleted()
                .FirstOrDefault(d => d.Name == newCommonProductProperty.Name && d.Measure == newCommonProductProperty.Measure);

            if (deletedCommonProperty != null)
            {
                newCommonProductProperty = deletedCommonProperty;
                this._commonProductDetailsRepository.Undelete(deletedCommonProperty);
            }
            else
            {
                await this._commonProductDetailsRepository.AddAsync(newCommonProductProperty);
            }

            await this._commonProductDetailsRepository.SaveChangesAsync();
            commonProductDetail = AutoMapper.Mapper.Map<TViewModel>(newCommonProductProperty);
            return commonProductDetail;
        }

        public async Task<bool> DeleteCommonProductDetailAsync(int productDetailId)
        {
            var commonDetailFromDb = await this._commonProductDetailsRepository.GetByIdWithDeletedAsync(productDetailId);
            if (commonDetailFromDb == null)
            {
                return false;
            }

            this._commonProductDetailsRepository.Delete(commonDetailFromDb);
            await this._commonProductDetailsRepository.SaveChangesAsync();
            return true;
        }

        public IQueryable<TViewModel> GetAllCommonProductDetails<TViewModel>()
        {
            return this._commonProductDetailsRepository.AllAsNoTracking()
                .Include(d => d.ProductDetails)
                .To<TViewModel>();
        }

        public bool IsCommonProductDetailExist(string productDetailName, string measure)
        {
            var exist = this._commonProductDetailsRepository.AllAsNoTracking()
                .Any(d => d.Name == productDetailName && d.Measure == measure);
            return exist;
        }

        public async Task<bool> UpdateCommonProductDetailAsync<TViewModel>(TViewModel commonProductDetail)
        {
            var appEntity = AutoMapper.Mapper.Map<CommonProductDetail>(commonProductDetail);

            var commonProductDetailFromDb = await this._commonProductDetailsRepository.GetByIdWithDeletedAsync(appEntity.Id);
            if (commonProductDetailFromDb == null)
            {
                return false;
            }

            AutoMapper.Mapper.Map(commonProductDetail, commonProductDetailFromDb);

            this._commonProductDetailsRepository.Update(commonProductDetailFromDb);
            await this._commonProductDetailsRepository.SaveChangesAsync();
            return true;
        }
    }
}
