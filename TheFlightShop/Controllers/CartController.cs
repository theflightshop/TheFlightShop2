using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheFlightShop.Models;
using Microsoft.AspNetCore.Mvc;
using TheFlightShop.Email;
using TheFlightShop.DAL;
using Microsoft.Extensions.Configuration;
using TheFlightShop.Payment;
using System.Net;

namespace TheFlightShop.Controllers
{
    public class CartController : Controller
    {
        private readonly IEmailClient _emailClient;
        private readonly IProductDAL _productDAL;
        private readonly IOrderDAL _orderDAL;
        private readonly NmiPaymentGateway _paymentGateway;

        public CartController(IEmailClient emailClient, IProductDAL productDal, IOrderDAL orderDal, NmiPaymentGateway paymentGateway)
        {
            _emailClient = emailClient;
            _productDAL = productDal;
            _orderDAL = orderDal;
            _paymentGateway = paymentGateway;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }

        public async Task<IActionResult> SubmitCustomerInfo(ClientOrder order)
        {
            var parts = await _productDAL.GetParts();
            var savedOrder = _orderDAL.SaveNewOrder(order, parts);

            IActionResult result;
            if (savedOrder)
            {
                var redirectUrl = $"{Request.Scheme}://{Request.Host}/Cart/{nameof(SubmitOrder)}";
                var gatewayFormUrlResult = await _paymentGateway.RetrievePaymentAuthUrl(order, parts, redirectUrl);
                var ifSuccessResult = new ContentResult
                {
                    Content = gatewayFormUrlResult.PaymentAuthFormUrl,
                    ContentType = "text/plain",
                    StatusCode = 200
                };
                result = GetPaymentGatewayActionResult(gatewayFormUrlResult, ifSuccessResult);
            }
            else
            {
                result = StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return result;
        }

        public async Task<IActionResult> SubmitOrder([FromQuery(Name = "token-id")]string tokenId)
        {
            var authResult = await _paymentGateway.AuthorizeOrValidatePaymentAmount(tokenId);
            var ifSuccessResult = new NoContentResult();
            var actionResult = GetPaymentGatewayActionResult(authResult, ifSuccessResult);
            return actionResult;
        }

        private IActionResult GetPaymentGatewayActionResult(PaymentGatewayResult gatewayResult, IActionResult ifSuccessResult)
        {
            IActionResult result;

            if (gatewayResult.Succeeded)
            {
                result = ifSuccessResult;
            }
            else if (gatewayResult.CanRetry)
            {
                result = new ContentResult
                {
                    Content = gatewayResult.ErrorReason,
                    ContentType = "text/plain",
                    StatusCode = 400
                };
            }
            else
            {
                result = StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return result;
        }
    }
}