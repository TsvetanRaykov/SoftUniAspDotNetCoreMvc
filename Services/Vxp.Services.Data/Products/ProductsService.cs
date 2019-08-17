namespace Vxp.Services.Data.Products
{
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using Vxp.Data.Common.Repositories;
    using Vxp.Data.Models;
    using Mapping;

    public class ProductsService : IProductsService
    {
        private readonly IDeletableEntityRepository<Product> _productsRepository;
        private readonly IRepository<ProductImage> _productImagesRepository;
        private readonly IDeletableEntityRepository<ProductCategory> _categoriesRepository;

        public ProductsService(IDeletableEntityRepository<Product> productsRepository,
            IRepository<ProductImage> productImagesRepository,
            IDeletableEntityRepository<ProductCategory> categoriesRepository)
        {
            this._productsRepository = productsRepository;
            this._productImagesRepository = productImagesRepository;
            this._categoriesRepository = categoriesRepository;
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

            newProduct.Category = await this._categoriesRepository.All().FirstOrDefaultAsync(c => c.Name == newProduct.Category.Name);
            await this._productImagesRepository.AddAsync(newProduct.Image);
            await this._productImagesRepository.SaveChangesAsync();
            await this._productsRepository.AddAsync(newProduct);
            await this._productsRepository.SaveChangesAsync();
            newProduct.Image.ProductId = newProduct.Id;
            this._productImagesRepository.Update(newProduct.Image);
            await this._productImagesRepository.SaveChangesAsync();

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