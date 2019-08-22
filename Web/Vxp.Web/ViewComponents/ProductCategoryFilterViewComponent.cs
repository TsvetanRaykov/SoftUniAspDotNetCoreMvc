using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vxp.Data.Common.Repositories;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewComponents
{
    public class ProductCategoryFilterViewComponent : ViewComponent
    {
        private readonly IDeletableEntityRepository<ProductCategory> _productCategoriesRepository;
        public ProductCategoryFilterViewComponent(IDeletableEntityRepository<ProductCategory> productCategoriesRepository)
        {
            this._productCategoriesRepository = productCategoriesRepository;
        }

        public IViewComponentResult Invoke(int categoryId)
        {
            var categories = this._productCategoriesRepository.AllAsNoTracking().To<SelectListItem>().ToList();
            categories.Add(new SelectListItem("- All -", "0"));
            foreach (var selectListItem in categories
                .Where(selectListItem => selectListItem.Value == categoryId.ToString()))
            {
                selectListItem.Selected = true;
                break;
            }

            return this.View(categories);
        }

    }
}