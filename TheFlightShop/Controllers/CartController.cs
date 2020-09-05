using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheFlightShop.Models;
using Microsoft.AspNetCore.Mvc;
using TheFlightShop.Email;
using TheFlightShop.DAL;
using Microsoft.Extensions.Logging;

namespace TheFlightShop.Controllers
{
    public class CartController : Controller
    {
        private readonly IEmailClient _emailClient;
        private readonly IProductDAL _productDAL;
        private readonly IOrderDAL _orderDAL;
        
        public CartController(IEmailClient emailClient, IProductDAL productDal, IOrderDAL orderDal)
        {
            _emailClient = emailClient;
            _productDAL = productDal;
            _orderDAL = orderDal;
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
            var parts = await _productDAL.GetParts();
            var succeeded = await _orderDAL.SaveNewOrder(order, parts);
            if (succeeded)
            {
                succeeded = await _emailClient.SendOrderConfirmation(order);
            }            
            return succeeded ? new OkResult() : new StatusCodeResult(400);
        }
    }
}