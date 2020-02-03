using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheFlightShop.Auth;

namespace TheFlightShop.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHash _hash;
        private readonly IToken _token;

        public AuthController(IHash hash, IToken token)
        {
            _hash = hash;
            _token = token;
        }

        [HttpPost]
        public IActionResult Login(LoginRequest loginRequest)
        {
            var adminHash = Environment.GetEnvironmentVariable("ADMIN_HASH");
            var adminSalt = Environment.GetEnvironmentVariable("ADMIN_SALT");
            var passwordMatches = _hash.ValueMatches(loginRequest.Password, adminHash, adminSalt);

            IActionResult result;
            if (passwordMatches)
            {
                var token = _token.GenerateToken(loginRequest.Username, Token.DEFAULT_HOURS_TO_EXPIRE);
                result = new JsonResult(new { token });
            }
            else
            {
                result = new UnauthorizedResult();
            }
            return result;
        }
    }
}