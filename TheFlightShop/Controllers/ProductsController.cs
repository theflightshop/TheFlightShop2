using System;
using Microsoft.AspNetCore.Mvc;
using TheFlightShop.DAL;

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
        public IActionResult Index()
        {
            var viewModel = _productReadDAL.GetProductCategories();
            return View(viewModel);
        }

        public IActionResult Category(Guid id)
        {
            var viewModel = _productReadDAL.GetProducts(id);
            return View(viewModel);
        }

        public IActionResult ProductDetail(Guid id)
        {
            var productView = _productReadDAL.GetProductView(id);
            return productView == null ? (IActionResult)new StatusCodeResult(404) : View(productView);
        }
    }
}
