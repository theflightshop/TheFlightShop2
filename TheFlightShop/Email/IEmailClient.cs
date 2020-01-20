using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheFlightShop.Models;

namespace TheFlightShop.Email
{
    public interface IEmailClient
    {
        Task<bool> SendOrderConfirmation(ClientOrder order);
    }
}
