using System.Collections.Generic;

namespace Vxp.Services.Data.Products
{
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using Vxp.Data.Common.Repositories;
    using Vxp.Data.Models;

    public class ProductsService : IProductsService
    {
        private readonly IDeletableEntityRepository<Product> _productsRepository;
        private readonly IRepository<ProductImage> _productImagesRepository;
        private readonly IRepository<ProductDetail> _productDetailsRepository;

        public ProductsService(IDeletableEntityRepository<Product> productsRepository,
            IRepository<ProductImage> productImagesRepository,
            IRepository<ProductDetail> productDetailsRepository)
        {
            this._productsRepository = productsRepository;
            this._productImagesRepository = productImagesRepository;
            this._productDetailsRepository = productDetailsRepository;
        }

        public async Task<bool> IsProductExist<TViewModel>(TViewModel product)
        {
            var aProduct = AutoMapper.Mapper.Map<Product>(product);

            var exist = await this._productsRepository.AllAsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == aProduct.Name && p.CategoryId == aProduct.CategoryId);

            return exist != null && exist.Id != aProduct.Id;
        }

        public async Task<TViewModel> CreateProductAsync<TViewModel>(TViewModel product)
        {
            var newProduct = AutoMapper.Mapper.Map<Product>(product);

            await this._productsRepository.AddAsync(newProduct);
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

        public async Task<TViewModel> UpdateProductAsync<TViewModel>(TViewModel product)
        {
            var newProduct = AutoMapper.Mapper.Map<Product>(product);

            var oldProduct = await this._productsRepository.AllWithDeleted()
                .Include(p => p.Images)
                .Include(p => p.Details)
                .FirstOrDefaultAsync(p => p.Id == newProduct.Id);

            oldProduct.CategoryId = newProduct.CategoryId;
            oldProduct.Description = newProduct.Description;
            oldProduct.Name = newProduct.Name;

            var imagesToRemove = new List<ProductImage>();

            foreach (var image in oldProduct.Images.Where(image => newProduct.Images.All(i => i.Id != image.Id)))
            {
                imagesToRemove.Add(image);
                this._productImagesRepository.Delete(image);
            }

            imagesToRemove.ForEach(i => oldProduct.Images.Remove(i));

            foreach (var image in newProduct.Images.Where(image => image.Id == 0))
            {
                oldProduct.Images.Add(image);
            }

            var detailsToRemove = oldProduct.Details.Where(details => newProduct.Details.All(i => i.Id != details.Id)).ToList();
            detailsToRemove.ForEach(i => this._productDetailsRepository.Delete(i));

            foreach (var detail in newProduct.Details)
            {
                if (detail.Id == 0)
                {
                    oldProduct.Details.Add(detail);
                }
            }

            this._productsRepository.Update(oldProduct);

            await this._productImagesRepository.SaveChangesAsync();

            return AutoMapper.Mapper.Map<TViewModel>(oldProduct);
        }
    }
}