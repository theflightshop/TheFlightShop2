using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheFlightShop.Models;
using Microsoft.AspNetCore.Mvc;
using TheFlightShop.Email;

namespace TheFlightShop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }

        public async Task<IActionResult> SubmitOrder(ClientOrder order)
        {
            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var succeeded = await new BasicEmail(apiKey).SendMail(order);
            return succeeded ? new OkResult() : new StatusCodeResult(400);
        }
    }
}