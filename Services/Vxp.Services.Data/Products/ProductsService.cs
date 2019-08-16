using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Vxp.Data.Common.Repositories;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Services.Data.Products
{
    public class ProductsService : IProductsService
    {
        private readonly IDeletableEntityRepository<Product> _productsRepository;

        public ProductsService(IDeletableEntityRepository<Product> productsRepository)
        {
            this._productsRepository = productsRepository;
        }

        public bool IsProductExist(string productName, string categoryName)
        {
            return this._productsRepository.AllAsNoTracking()
                .Include(p => p.Category)
                .Any(p => p.Name == productName && p.Category.Name == categoryName);
        }

        public async Task<TViewModel> CreateProductAsync<TViewModel>(TViewModel product)
        {
            var newProduct = AutoMapper.Mapper.Map<Product>(product);
            var deletedProduct = this._productsRepository.AllWithDeleted()
                .FirstOrDefault(x => x.Name == newProduct.Name);

            if (deletedProduct != null)
            {
                newProduct = deletedProduct;
                this._productsRepository.Undelete(deletedProduct);
            }
            else
            {
                await this._productsRepository.AddAsync(newProduct);
            }

            await this._productsRepository.SaveChangesAsync();
            product = AutoMapper.Mapper.Map<TViewModel>(newProduct);
            return product;
        }

        public IQueryable<TViewModel> GetAllProducts<TViewModel>()
        {
            return this._productsRepository.AllAsNoTracking()
                .Include(p => p.Details)
                .Include(p => p.Images)
                .To<TViewModel>();
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var productFromDb = await this._productsRepository.GetByIdWithDeletedAsync(productId);
            if (productFromDb == null)
            {
                return false;
            }

            this._productsRepository.Delete(productFromDb);
            await this._productsRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProductAsync<TViewModel>(TViewModel product)
        {
            var appEntity = AutoMapper.Mapper.Map<Product>(product);

            var productFromDb = await this._productsRepository.GetByIdWithDeletedAsync(appEntity.Id);
            if (productFromDb == null)
            {
                return false;
            }

            AutoMapper.Mapper.Map(product, productFromDb);

            this._productsRepository.Update(productFromDb);
            await this._productsRepository.SaveChangesAsync();
            return true;
        }
    }
}