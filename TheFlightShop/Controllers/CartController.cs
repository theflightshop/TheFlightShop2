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

        public async Task<IActionResult> SubmitAlternatePaymentOrder(ClientOrder order)
        {
            var parts = await _productDAL.GetParts();
            order.GenerateConfirmationNumber();
            var savedOrder = await _orderDAL.SaveNewOrder(order, parts);

            IActionResult actionResult;
            if (savedOrder)
            {
                var emailsSent = await _emailClient.SendOrderConfirmation(order);
                if (emailsSent)
                {
                    ViewData["Title"] = "Order Submitted";
                    var submissionResult = CheckoutSubmissionViewModel.Success(order.ConfirmationNumber);
                    actionResult = Json(submissionResult);
                }
                else
                {
                    actionResult = StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                actionResult = StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return actionResult;
        }

        public async Task<IActionResult> SubmitCustomerInfo(ClientOrder order)
        {
            var parts = await _productDAL.GetParts();
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/Cart/{nameof(SubmitOrder)}";
            var gatewayFormUrlResult = await _paymentGateway.RetrievePaymentAuthUrl(order, parts, redirectUrl);

            IActionResult result;
            if (gatewayFormUrlResult.Succeeded)
            {
                order.ConfirmationNumber = gatewayFormUrlResult.TransactionId;
                var savedOrder = await _orderDAL.SaveNewOrder(order, parts);
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
                        var submissionResult = CheckoutSubmissionViewModel.Success(clientOrder.ConfirmationNumber);
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