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
using Microsoft.Extensions.Logging;

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
            var savedOrder = await _orderDAL.SaveNewOrder(order, parts);

            IActionResult result;
            if (savedOrder)
            {
                var redirectUrl = $"{Request.Scheme}://{Request.Host}/Cart/{nameof(SubmitOrder)}";
                var gatewayFormUrlResult = await _paymentGateway.RetrievePaymentAuthUrl(order, parts, redirectUrl);
                if (gatewayFormUrlResult.Succeeded)
                {
                    result = new ContentResult
                    {
                        Content = gatewayFormUrlResult.PaymentAuthFormUrl,
                        ContentType = "text/plain",
                        StatusCode = 200
                    };
                }
                else
                {
                    result = StatusCode((int)HttpStatusCode.InternalServerError);
                }
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
            IActionResult actionResult;

            if (authResult.Succeeded)
            {
                var clientOrder = ClientOrder.FromNmiGatewayResponse(authResult);
                if (clientOrder == null)
                {
                    actionResult = new RedirectToActionResult("Index", "Home", new { orderSubmitted = false });
                }
                else
                {
                    var emailsSent = await _emailClient.SendOrderConfirmation(clientOrder);
                    if (emailsSent)
                    {
                        ViewData["Title"] = "Order Submitted";
                        var submissionResult = CheckoutSubmissionViewModel.Success(authResult.ConfirmationNumber);
                        actionResult = View("Checkout", submissionResult);
                    }
                    else
                    {
                        actionResult = new RedirectToActionResult("Index", "Home", new { orderSubmitted = false });
                    }
                }
            }
            else
            {
                ViewData["Title"] = "Error Submitting Order";
                var submissionResult = CheckoutSubmissionViewModel.Failure(authResult.ErrorReason);
                actionResult = View("Checkout", submissionResult);
            }
            return actionResult;
        }
    }
}