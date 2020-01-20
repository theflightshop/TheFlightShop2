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
        private IEmailClient _emailClient;

        public CartController(IEmailClient emailClient)
        {
            _emailClient = emailClient;
        }

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
            var succeeded = await _emailClient.SendOrderConfirmation(order);
            return succeeded ? new OkResult() : new StatusCodeResult(400);
        }
    }
}