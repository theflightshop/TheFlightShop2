using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Models
{
    public class PaymentGatewayFormUrlResult : PaymentGatewayResult
    {
        public string PaymentAuthFormUrl { get; set; }
    }
}
