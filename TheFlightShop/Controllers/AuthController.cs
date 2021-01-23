using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheFlightShop.Auth;
using TheFlightShop.Logging;

namespace TheFlightShop.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHash _hash;
        private readonly IToken _token;
        private readonly ILogger _logger;

        public AuthController(IHash hash, IToken token, ILogger<AuthController> logger)
        {
            _hash = hash;
            _token = token;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Login(LoginRequest loginRequest)
        {
            try
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
            catch (Exception ex)
            {
                throw new FlightShopActionException($"{nameof(AuthController)}.{nameof(Login)}- Error logging in with username={loginRequest?.Username}.", ex);
            }
        }
    }
}