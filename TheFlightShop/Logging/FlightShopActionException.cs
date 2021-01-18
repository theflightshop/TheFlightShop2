using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Logging
{
    /// <summary>
    /// Represents exception caught within controller-action context and then thrown so global logic may handle it. 
    /// This is the quick and dirty solution versus implementing specific error-handling in each scope of the logic/application. 
    /// </summary>
    public class FlightShopActionException : Exception
    {
        public FlightShopActionException(string message, Exception ex) : base(message, ex) { }
    }
}
