using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Auth
{
    public interface IToken
    {
        string GenerateToken(string username, int hoursToExpire);
        TokenValidationResult ValidateToken(string token, string expectedUsername = null);
    }
}
