namespace Vxp.Services.Data.Products
{
    using System.Threading.Tasks;
    using System.Linq;
    using Vxp.Data.Common.Repositories;
    using Vxp.Data.Models;
    using Mapping;

    public class ProductCategoriesService : IProductCategoriesService
    {
        private readonly IDeletableEntityRepository<ProductCategory> _productCategoriesRepository;

        public ProductCategoriesService(IDeletableEntityRepository<ProductCategory> productCategoriesRepository)
        {
            this._productCategoriesRepository = productCategoriesRepository;
        }

        public bool IsCategoryExist(string categoryName)
        {
            var categoryFromDb = this._productCategoriesRepository
                .AllAsNoTracking()
                .FirstOrDefault(c => c.Name == categoryName);

            return categoryFromDb != null;
        }

        public async Task<TViewModel> CreateCategoryAsync<TViewModel>(TViewModel productCategory)
        {
            var newProductCategory = AutoMapper.Mapper.Map<ProductCategory>(productCategory);

            var deletedCategory = this._productCategoriesRepository.AllWithDeleted()
                .FirstOrDefault(x => x.Name == newProductCategory.Name);

            if (deletedCategory != null)
            {
                newProductCategory = deletedCategory;
                this._productCategoriesRepository.Undelete(deletedCategory);
            }
            else
            {
                await this._productCategoriesRepository.AddAsync(newProductCategory);
            }

            await this._productCategoriesRepository.SaveChangesAsync();
            productCategory = AutoMapper.Mapper.Map<TViewModel>(newProductCategory);
            return productCategory;
        }

        public IQueryable<TViewModel> GetAllCategories<TViewModel>()
        {
            return this._productCategoriesRepository.AllAsNoTracking().To<TViewModel>();
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var categoryFromDb = await this._productCategoriesRepository.GetByIdWithDeletedAsync(categoryId);
            if (categoryFromDb == null)
            {
                return false;
            }

            this._productCategoriesRepository.Delete(categoryFromDb);
            await this._productCategoriesRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, string newName)
        {
            var categoryFromDb = await this._productCategoriesRepository.GetByIdWithDeletedAsync(categoryId);
            if (categoryFromDb == null)
            {
                return false;
            }

            categoryFromDb.Name = newName;

            this._productCategoriesRepository.Update(categoryFromDb);
            await this._productCategoriesRepository.SaveChangesAsync();
            return true;
        }
    }
}