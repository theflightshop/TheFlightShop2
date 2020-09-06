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
        Task<bool> SaveNewOrder(ClientOrder clientOrder, IEnumerable<Part> parts);
    }
}
