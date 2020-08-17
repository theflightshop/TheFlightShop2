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
        private readonly ILogger _logger;

        public CartController(IEmailClient emailClient, IProductDAL productDal, IOrderDAL orderDal, ILogger<CartController> logger)
        {
            _emailClient = emailClient;
            _productDAL = productDal;
            _orderDAL = orderDal;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogWarning("Testing some cute logging functionality ;)");
            return View();
        }

        public IActionResult Checkout()
        {
            _logger.LogError(new Exception("this is a test error message"), "this is the only time an exception is good");
            return View();
        }

        public async Task<IActionResult> SubmitOrder(ClientOrder order)
        {
            var parts = await _productDAL.GetParts();
            var succeeded = _orderDAL.SaveNewOrder(order, parts);
            if (succeeded)
            {
                succeeded = await _emailClient.SendOrderConfirmation(order);
            }            
            return succeeded ? new OkResult() : new StatusCodeResult(400);
        }
    }
}