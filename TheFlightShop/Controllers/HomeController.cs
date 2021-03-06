using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheFlightShop.DAL;
using TheFlightShop.Email;
using TheFlightShop.Models;
using TheFlightShop.Weather;

namespace TheFlightShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductDAL _productReadDAL;
        private readonly IWeatherClient _weatherClient;
        private readonly IEmailClient _emailClient;

        public HomeController(IProductDAL productReadDAL, IWeatherClient weatherClient, IEmailClient emailClient)
        {
            _productReadDAL = productReadDAL;
            _weatherClient = weatherClient;
            _emailClient = emailClient;
        }

        public IActionResult Index(bool? orderSubmitted = null)
        {
            var result = new HomeViewModel
            {
                OrderSubmitted = orderSubmitted.HasValue,
                OrderSubmissionFailed = orderSubmitted.HasValue && !orderSubmitted.Value
            };

            var homepageMessage = Environment.GetEnvironmentVariable("HOMEPAGE_MESSAGE");
            if (!string.IsNullOrWhiteSpace(homepageMessage))
            {
                var showMessageThruText = Environment.GetEnvironmentVariable("HOMEPAGE_MSG_DISPLAY_THRU_DATE_YYYYMMDD");
                DateTime showMessageThruDate;
                if (DateTime.TryParseExact(showMessageThruText, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out showMessageThruDate) && showMessageThruDate >= DateTime.UtcNow.Date)
                {
                    result.AlertMessage = homepageMessage;
                }
            }

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
        public IActionResult Error(string id)
        {
            return View(new ErrorViewModel { Id = id });
        }

        public async Task<IActionResult> SubmitCustomerInfoOnError(ErrorCustomerInfo customerInfo)
        {
            if (string.IsNullOrWhiteSpace(customerInfo.Email)) 
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var emailBody = $@"
<span><strong>Error ID:</strong> {customerInfo.ErrorId} <em>(This ID should be visible in separate email that lists error messages, and in storage of detailed error descriptions, which should be available to your IT admin)</em></span><br />
<span><strong>Customer Email:</strong> {customerInfo.Email}</span><br />
<span><strong>Name:</strong> {customerInfo.Name}</span><br />
<span><strong>Phone:</strong> {customerInfo.Phone}</span><br />
";
                await _emailClient.SendEmail($"ERROR REPORT - customer info submitted (Error ID {customerInfo.ErrorId})", emailBody);
                return View();
            }
        }
    }
}
