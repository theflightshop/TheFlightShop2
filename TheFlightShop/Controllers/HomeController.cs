using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheFlightShop.DAL;
using TheFlightShop.Models;

namespace TheFlightShop.Controllers
{
    public class HomeController : Controller
    {
        private IProductReadDAL _productReadDAL;

        public HomeController(IProductReadDAL productReadDAL)
        {
            _productReadDAL = productReadDAL;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        #region Maintenance

        public IActionResult CompanyProfile()
        {
            return View();
        }

        public IActionResult Facility()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Parts()
        {
            return View();
        }

        public IActionResult Modifications()
        {
            return View();
        }

        public IActionResult CabinBaggageDoorStruts()
        {
            return View();
        }

        public IActionResult CabinDoorCables()
        {
            return View();
        }

        public IActionResult WindowShades()
        {
            return View();
        }

        public IActionResult InstrumentPanels()
        {
            return View();
        }

        #endregion

        #region Click Bond

        public async Task<IActionResult> ClickBondAuthorizedDistributor()
        {
            return await Task.Run(() =>
            {
                var viewModel = _productReadDAL.GetProductCategories();
                return View(viewModel);
            });
        }

        public IActionResult ClickBondInstallation()
        {
            return View();
        }

        public IActionResult ClickBondStrengthData()
        {
            return View();
        }

        public IActionResult OrderingInformation()
        {
            return View();
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
