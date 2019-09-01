using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vxp.Data.Models;
using Vxp.Data.Repositories;
using Vxp.Services.Data.Products;
using Vxp.Web.ViewModels.Vendor.Products;
using Xunit;

namespace Vxp.Services.Data.Tests
{
    [Collection("Database collection")]
    public class ProductsServiceTests
    {
        private readonly DatabaseFixture _fixture;
        private readonly IProductsService _productsService;

        public ProductsServiceTests(DatabaseFixture fixture)
        {
            this._fixture = fixture;
            this._productsService = this.GetProductsService();
        }

        [Fact]
        public async Task UpdateProductNameTest()
        {
            Mapping.Config(typeof(ProductInputModel),
                typeof(ProductImageInputModel),
                typeof(ProductCategoryInputModel));

            var newProduct = new Product
            {
                Name = "Test Product Exist",
                Image = new ProductImage(),
                Category = new ProductCategory { Name = "Test Category" }
            };

            var productsRepository = new EfDeletableEntityRepository<Product>(this._fixture.DbContext);

            await productsRepository.AddAsync(newProduct);
            await productsRepository.SaveChangesAsync();
            var model = AutoMapper.Mapper.Map<ProductInputModel>(newProduct);
            var newName = Guid.NewGuid().ToString();
            model.Name = newName;
            model.Image = new ProductImageInputModel();
            await this._productsService.UpdateProductAsync(model);
            var updatedProduct = await productsRepository.GetByIdWithDeletedAsync(newProduct.Id);
            Assert.Equal(newName, updatedProduct.Name);
        }

        [Fact]
        public async Task GetDeletedProductsTest()
        {
            Mapping.Config(typeof(ProductInputModel),
                typeof(ProductImageInputModel),
                typeof(ProductCategoryInputModel),
                    typeof(ProductDetailInputModel),
                typeof(ProductCommonDetailInputModel));

            var newProduct = new Product
            {
                IsDeleted = true,
                Name = "Test Product Exist",
                Image = new ProductImage(),
                Category = new ProductCategory { Name = "Test Category" }
            };

            var productsRepository = new EfDeletableEntityRepository<Product>(this._fixture.DbContext);

            await productsRepository.AddAsync(newProduct);
            await productsRepository.SaveChangesAsync();


            var deletedProduct = this._productsService.GetDeletedProducts<ProductInputModel>()
                .GetAwaiter().GetResult().FirstOrDefault(p => p.Id == newProduct.Id);

            Assert.NotNull(deletedProduct);
        }

        [Fact]
        public async Task GetAllProductsTest()
        {
            Mapping.Config(typeof(ProductInputModel),
                typeof(ProductImageInputModel),
                typeof(ProductCategoryInputModel),
                typeof(ProductDetailInputModel),
                typeof(ProductCommonDetailInputModel));

            var productsRepository = new EfDeletableEntityRepository<Product>(this._fixture.DbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(this._fixture.DbContext);
            var user = await usersRepository.AllWithDeleted().FirstOrDefaultAsync();

            for (int i = 0; i < 10; i++)
            {
                await productsRepository.AddAsync(new Product
                {
                    IsDeleted = false,
                    Name = $"Test Product{i}",
                    Image = new ProductImage(),
                    Category = new ProductCategory { Name = $"Test Category{i}" }
                });
            }

            await productsRepository.SaveChangesAsync();

            var products = await this._productsService.GetAllProducts<ProductInputModel>().ToArrayAsync();

            Assert.True(10 == products.Length);
        }


        [Fact]
        public async Task UpdateProductImagesTest()
        {
            Mapping.Config(typeof(ProductInputModel),
                typeof(ProductImageInputModel),
                typeof(ProductCategoryInputModel));

            var newProduct = new Product
            {
                Name = "Test Product Exist",
                Image = new ProductImage(),
                Category = new ProductCategory { Name = "Test Category" },
            };

            newProduct.Images.Add(new ProductImage());

            var productsRepository = new EfDeletableEntityRepository<Product>(this._fixture.DbContext);

            await productsRepository.AddAsync(newProduct);
            await productsRepository.SaveChangesAsync();
            var model = AutoMapper.Mapper.Map<ProductInputModel>(newProduct);
            var newName = Guid.NewGuid().ToString();
            model.Name = newName;
            model.Image = null;
            model.Images.Add(new ProductImageInputModel());

            await this._productsService.UpdateProductAsync(model);
            var updatedProduct = await productsRepository.GetByIdWithDeletedAsync(newProduct.Id);
            Assert.Equal(newName, updatedProduct.Name);
        }

        [Fact]
        public async Task DeleteProductAsyncShouldDeleteProduct()
        {
            Mapping.Config(typeof(ProductInputModel),
                typeof(ProductImageInputModel),
                typeof(ProductCategoryInputModel));

            var newProduct = new Product
            {
                Name = "Test Product Exist",
                Image = new ProductImage(),
                Category = new ProductCategory { Name = "Test Category" },
            };
            newProduct.Details.Add(new ProductDetail());
            newProduct.Images.Add(new ProductImage());

            var productsRepository = new EfDeletableEntityRepository<Product>(this._fixture.DbContext);

            await productsRepository.AddAsync(newProduct);
            await productsRepository.SaveChangesAsync();

            await this._productsService.DeleteProductAsync(newProduct.Id);

            var product = await productsRepository.AllAsNoTracking().FirstOrDefaultAsync(p => p.Id == newProduct.Id);

            Assert.Null(product);
        }

        [Fact]
        public async Task DeleteProductPermanentlyTest()
        {
            Mapping.Config(typeof(ProductInputModel),
                typeof(ProductImageInputModel),
                typeof(ProductCategoryInputModel));

            var newProduct = new Product
            {
                Name = "Test Product Exist",
                Image = new ProductImage(),
                Category = new ProductCategory { Name = "Test Category" },
            };
            newProduct.Details.Add(new ProductDetail());
            newProduct.Images.Add(new ProductImage());

            var productsRepository = new EfDeletableEntityRepository<Product>(this._fixture.DbContext);

            await productsRepository.AddAsync(newProduct);
            await productsRepository.SaveChangesAsync();

            await this._productsService.DeletePermanentlyAsync(newProduct.Id);

            var product = await productsRepository.AllWithDeleted().FirstOrDefaultAsync(p => p.Id == newProduct.Id);

            Assert.Null(product);
        }

        [Fact]
        public async Task DeleteProductPermanentlyShouldReturnFalseIfNotFound()
        {
            var result = await this._productsService.DeletePermanentlyAsync(int.MaxValue);
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteProductAsyncShouldReturnFalseIfProductNotFound()
        {
            var result = await this._productsService.DeleteProductAsync(int.MaxValue);
            Assert.False(result);
        }

        [Fact]
        public async Task RestoreProductTest()
        {
            var newProduct = new Product
            {
                Name = "Test Product Exist",
                Image = new ProductImage(),
                Category = new ProductCategory { Name = "Test Category" },
                IsDeleted = true
            };

            var productsRepository = new EfDeletableEntityRepository<Product>(this._fixture.DbContext);

            await productsRepository.AddAsync(newProduct);
            await productsRepository.SaveChangesAsync();

            await this._productsService.RestoreAsync(newProduct.Id);

            Assert.False(newProduct.IsDeleted);
        }

        [Fact]
        public async Task IsProductExistShouldReturnTrueIfProductExist()
        {
            Mapping.Config(typeof(ProductInputModel),
                typeof(ProductImageInputModel),
                typeof(ProductCategoryInputModel));

            var newProduct = new Product
            {
                Name = "Test Product Exist",
                Image = new ProductImage(),
                Category = new ProductCategory { Name = "Test Category" }
            };

            var productsRepository = new EfDeletableEntityRepository<Product>(this._fixture.DbContext);

            await productsRepository.AddAsync(newProduct);
            await productsRepository.SaveChangesAsync();

            var model = AutoMapper.Mapper.Map<ProductInputModel>(newProduct);
            model.Id = null;

            var productExist = await this._productsService.IsProductExist(model);

            Assert.True(productExist);

        }


        [Fact]
        public async Task CreateProductShouldReturnNewProduct()
        {
            Mapping.Config(typeof(ProductInputModel),
                typeof(ProductImageInputModel),
                    typeof(ProductCategoryInputModel));

            var categoryRepository = new EfDeletableEntityRepository<ProductCategory>(this._fixture.DbContext);
            var newProductCategory = new ProductCategory
            {
                Name = "TestProductCategory"
            };
            await categoryRepository.AddAsync(newProductCategory);
            await categoryRepository.SaveChangesAsync();
            var newProductModel = new ProductInputModel
            {
                CategoryId = newProductCategory.Id,
                Image = new ProductImageInputModel()
            };
            var newProduct = await this._productsService.CreateProductAsync(newProductModel);
            Assert.NotNull(newProduct);
        }

        private IProductsService GetProductsService()
        {
            var productsRepository = new EfDeletableEntityRepository<Product>(this._fixture.DbContext);
            var imagesRepository = new EfRepository<ProductImage>(this._fixture.DbContext);
            var detailsRepository = new EfRepository<ProductDetail>(this._fixture.DbContext);
            return new ProductsService(productsRepository, imagesRepository, detailsRepository);
        }
    }
}