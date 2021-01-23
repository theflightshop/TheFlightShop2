using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheFlightShop.Models;

namespace TheFlightShop.Email
{
    public interface IEmailClient
    {
        /// <summary>
        /// Send confirmation email of customer order to default address, typically of system admin or company admin. 
        /// </summary>
        Task<bool> SendOrderConfirmation(ClientOrder order);

        /// <summary>
        /// Send email to specified address.
        /// </summary>
        Task<bool> SendEmail(string toAddress, string subject, string body);

        /// <summary>
        /// Send email to default address, typically of system admin or company admin. 
        /// </summary>
        Task<bool> SendEmail(string subject, string body);
    }
}
