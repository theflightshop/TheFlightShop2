using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TheFlightShop.Auth
{
    public class Token : IToken
    {
        public const int DEFAULT_HOURS_TO_EXPIRE = 24;

        private readonly string _issuer;
        private readonly string _audience;
        private readonly byte[] _signingKey;

        public Token(string issuer, string audience, string signingKey)
        {
            _issuer = issuer;
            _audience = audience;
            _signingKey = Encoding.ASCII.GetBytes(signingKey);
        }

        public string GenerateToken(string username, int hoursToExpire)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenInfo = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, username)
                }),
                Audience = _audience,
                Issuer = _issuer,
                Expires = DateTime.UtcNow.AddHours(hoursToExpire),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_signingKey), SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenInfo);
            return tokenHandler.WriteToken(token);
        }

        public TokenValidationResult ValidateToken(string token, string expectedUsername = null)
        {
            var validationParams = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(_signingKey),
                ValidAudience = _audience,
                ValidIssuer = _issuer
            };
            var validationResult = new TokenValidationResult { Valid = false, Expired = false };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParams, out validatedToken);

                var validated = validatedToken != null && principal != null;
                if (validated)
                {
                    var expired = validatedToken.ValidTo < DateTime.UtcNow;
                    var isMatchingUser = expectedUsername == null || principal.Identity.Name == expectedUsername;
                    validationResult.Valid = isMatchingUser;
                    validationResult.Expired = expired;
                }
            }
            catch (Exception ex)
            {
                if (ex is SecurityTokenValidationException || ex is ArgumentException)
                {
                    // log exception, dont throw because this exception means validation failed (cheers to Microsoft for deliberately throwing exceptions)
                }
                else throw;
            }

            return validationResult;
        }
    }
}
