using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheFlightShop.DAL;
using TheFlightShop.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TheFlightShop.Controllers
{
    public class ProductsController : Controller
    {
        private IProductReadDAL _productReadDAL;

        public ProductsController(IProductReadDAL productReadDAL)
        {
            _productReadDAL = productReadDAL;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(Guid? categoryId = null)
        {
            return await Task.Run(() =>
            {
                var productCounts = (IEnumerable<ProductCategoryCount>)null;
                if (categoryId.HasValue)
                {
                    var productCount = _productReadDAL.GetProductCategoryCount(categoryId.Value);
                    productCounts = new List<ProductCategoryCount> { productCount };
                }
                else
                {
                    productCounts = _productReadDAL.GetProductCategoryCounts();
                }
                return View(productCounts);
            });
        }

        public async Task<IActionResult> SubCategory(Guid id)
        {
            return await Task.Run(() =>
            {
                var subCategoryView = _productReadDAL.GetSubCategoryView(id);
                return View(subCategoryView);
            });
        }
    }
}
