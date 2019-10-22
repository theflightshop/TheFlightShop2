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
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() =>
            {
                var viewModel = _productReadDAL.GetProductCategories();
                return View(viewModel);
            });
        }

        public async Task<IActionResult> Category(Guid id)
        {
            return await Task.Run(() =>
            {
                var viewModel = _productReadDAL.GetProducts(id);
                return View(viewModel);
            });
        }

        public async Task<IActionResult> ProductDetail(Guid id)
        {
            return await Task.Run(() =>
            {
                var productView = _productReadDAL.GetProductView(id);
                return productView == null ? (IActionResult)new StatusCodeResult(404) : View(productView);
            });
        }
    }
}
