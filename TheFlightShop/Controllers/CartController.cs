using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheFlightShop.Models;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult SubmitOrder(ClientOrder order)
        {
            return new JsonResult("neato");
        }
    }
}