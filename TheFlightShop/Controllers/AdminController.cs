using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TheFlightShop.Auth;
using TheFlightShop.DAL;
using TheFlightShop.Models;

namespace TheFlightShop.Controllers
{
    //[TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
    public class AdminController : Controller
    {
        private readonly IHash _hash;
        private readonly IToken _token;
        private readonly IProductReadDAL _productReadDal;

        public AdminController(IHash hash, IToken token, IProductReadDAL productReadDal)
        {
            _hash = hash;
            _token = token;
            _productReadDal = productReadDal;
        }

        public IActionResult Encrypt(string value)
        {
            var hashAndSalt = _hash.GenerateHashAndSalt(value);
            return new JsonResult(new { hash = hashAndSalt.Item1, salt = hashAndSalt.Item2 });
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Products()
        {
            var products = _productReadDal.GetAllProducts();
            return new JsonResult(new { products });
        }

        public IActionResult UpdateProduct([FromForm]string code, [FromForm]IFormFile image, [FromForm]IFormFile drawing)
        {
            var x = 0;
            return new OkResult();
        }

        [HttpPost]
        public IActionResult Login(LoginRequest loginRequest)
        {
            var adminUsername = Environment.GetEnvironmentVariable("ADMIN_USERNAME");
            var adminHash = Environment.GetEnvironmentVariable("ADMIN_HASH");
            var adminSalt = Environment.GetEnvironmentVariable("ADMIN_SALT");
            var passwordMatches = /*loginRequest.Username == adminUsername && */ _hash.ValueMatches(loginRequest.Password, adminHash, adminSalt);

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