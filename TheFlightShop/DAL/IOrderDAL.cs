using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheFlightShop.DAL.Schemas;
using TheFlightShop.Models;

namespace TheFlightShop.DAL
{
    public interface IOrderDAL
    {
        //todo: Task<ClientOrder> GetClientOrderByConfirmationNumber(string confirmationNumber);
        Task<bool> SaveNewOrder(ClientOrder clientOrder, IEnumerable<Part> parts);
    }
}
