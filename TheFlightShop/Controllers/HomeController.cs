using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheFlightShop.DAL;
using TheFlightShop.Models;
using TheFlightShop.Weather;

namespace TheFlightShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductDAL _productReadDAL;
        private readonly IWeatherClient _weatherClient;

        public HomeController(IProductDAL productReadDAL, IWeatherClient weatherClient)
        {
            _productReadDAL = productReadDAL;
            _weatherClient = weatherClient;
        }

        public IActionResult Index(bool? orderSubmitted = null)
        {
            var result = new HomeViewModel
            {
                OrderSubmitted = orderSubmitted.HasValue,
                OrderSubmissionFailed = orderSubmitted.HasValue && !orderSubmitted.Value
            };
            return View(result);
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

        public async Task<IActionResult> Search(string q)
        {
            var results = await _productReadDAL.SearchParts(q);
            return new JsonResult(new { results, query = q });
        }

        public async Task<IActionResult> SearchResults(string q)
        {
            var results = await _productReadDAL.SearchParts(q);
            var resultView = new SearchResultView
            {
                Query = q,
                Results = results
            };
            return View(resultView);
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

        #endregion

        #region Click Bond

        public async Task<IActionResult> ClickBondAuthorizedDistributor()
        {
            var viewModel = await _productReadDAL.GetProductCategories();
            return View(viewModel);
        }

        public IActionResult ClickBondInstallation()
        {
            return View();
        }

        public IActionResult ClickBondStrengthData()
        {
            return View();
        }

        #endregion

        public async Task<IActionResult> CurrentWeatherInfo()
        {
            var weather = await _weatherClient.GetWeather();
            return weather == null ? (IActionResult)new StatusCodeResult(500) : new JsonResult(weather);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
