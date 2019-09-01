using System.Threading.Tasks;
using Vxp.Data.Models;
using Vxp.Data.Repositories;
using Vxp.Services.Data.Products;
using Vxp.Web.ViewModels.Vendor.Products;
using Xunit;

namespace Vxp.Services.Data.Tests
{
    [Collection("Database collection")]
    public class ProductDetailsServiceTests
    {
        private readonly DatabaseFixture _fixture;
        private readonly IProductDetailsService _productDetailsService;

        public ProductDetailsServiceTests(DatabaseFixture fixture)
        {
            this._fixture = fixture;
            this._productDetailsService = GetProductDetailsService();
        }

        [Fact]
        public async Task CreateCommonDetailWithUndeleteTest()
        {
            Mapping.Config(typeof(ProductCommonDetailInputModel));

            var detailsRepository = new EfDeletableEntityRepository<CommonProductDetail>(this._fixture.DbContext);

            await detailsRepository.AddAsync(new CommonProductDetail
            {
                Name = "Test Common Detail",
                Measure = "Test Measure",
                IsDeleted = true
            });

            await detailsRepository.SaveChangesAsync();

            var detailModel = new ProductCommonDetailInputModel { Name = "Test Common Detail", Measure = "Test Measure" };

            var createDetail = await this._productDetailsService.CreateCommonProductDetailAsync(detailModel);

            Assert.True(createDetail.Id > 0);
        }

        [Fact]
        public async Task CreateCommonDetailNewDetailTest()
        {
            Mapping.Config(typeof(ProductCommonDetailInputModel));

            var detailModel = new ProductCommonDetailInputModel { Name = "Test Common Detail", Measure = "Test Measure" };

            var createDetail = await this._productDetailsService.CreateCommonProductDetailAsync(detailModel);

            Assert.True(createDetail.Id > 0);
        }

        private IProductDetailsService GetProductDetailsService()
        {
            var detailsRepository = new EfDeletableEntityRepository<CommonProductDetail>(this._fixture.DbContext);
            return new ProductDetailsService(detailsRepository);
        }
    }
}