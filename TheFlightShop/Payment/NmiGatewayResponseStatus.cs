using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Payment
{
    public enum NmiGatewayResponseStatus
    {
        Approved = 1,
        Declined = 2,
        Error = 3
    }
}
