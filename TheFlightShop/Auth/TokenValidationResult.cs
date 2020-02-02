using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Auth
{
    public class TokenValidationResult
    {
        public bool Valid { get; set; }
        public bool Expired { get; set; }
        public bool Ok => Valid && !Expired;
    }
}
