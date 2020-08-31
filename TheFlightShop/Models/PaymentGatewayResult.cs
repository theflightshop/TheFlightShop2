using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Models
{
    public class PaymentGatewayResult
    {
        public bool Succeeded { get; set; }
        public bool CanRetry { get; set; }
        public string ErrorReason { get; set; }
    }
}
