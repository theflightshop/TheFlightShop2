using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Auth
{
    public interface IHash
    {
        Tuple<string, string> GenerateHashAndSalt(string value);
        bool ValueMatches(string value, string hash, string salt);
    }
}
