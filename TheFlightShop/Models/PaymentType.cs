using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Models
{
    public enum PaymentType
    {
        [Description("Unknown")]
        Unknown = 0,
        [Description("Credit card")]
        CreditCard = 1,
        [Description("COD")]
        Cod = 2,
        [Description("On account (PO number)")]
        OnAccount = 3,
        [Description("ACH")]
        Ach = 4
    }
}
