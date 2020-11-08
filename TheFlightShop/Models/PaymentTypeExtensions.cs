using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Models
{
    public static class PaymentTypeExtensions
    {
        public static string GetDescription(this PaymentType paymentType)
        {
            var attributes = (DescriptionAttribute[])paymentType.GetType().GetField(paymentType.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
