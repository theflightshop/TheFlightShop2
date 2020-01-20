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

        public IActionResult SubmitOrder(ClientOrder order)
        {
            new BasicEmail().SendMail();
            return new JsonResult("neato");
        }

        public IActionResult TestMail()
        {
            new BasicEmail().SendMail();
            return new JsonResult("neato");
        }
    }
}