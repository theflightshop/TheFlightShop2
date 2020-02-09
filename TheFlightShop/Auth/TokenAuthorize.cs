using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
//using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
//using System.Web.Http.Results;
using System.Web.Mvc;

namespace TheFlightShop.Auth
{
    public class TokenAuthorize : Attribute, IResourceFilter // : System.Web.Mvc.AuthorizeAttribute//AuthorizeAttribute// System.Web.Http.Filters.ActionFilterAttribute, IAuthenticationFilter
    {
        public string[] Roles { get; set; }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var authorization = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorization?.ToString()))
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }
            else
            {
                var splitToken = authorization.ToString().Split("bearer ");
                var tokenValue = splitToken.Length == 2 ? splitToken[1] : null;
                if (string.IsNullOrEmpty(tokenValue))
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);
                }
                else
                {
                    var tokenIssuer = Environment.GetEnvironmentVariable("AUTH_ISSUER");
                    var tokenAudience = Environment.GetEnvironmentVariable("AUTH_AUDIENCE");
                    var signingKey = Environment.GetEnvironmentVariable("AUTH_KEY");
                    var tokenValidation = new Token(tokenIssuer, tokenAudience, signingKey);
                    var expectedUsername = IsAdminRoleRequired() ? Environment.GetEnvironmentVariable("ADMIN_USERNAME") : null;
                    var validationResult = tokenValidation.ValidateToken(tokenValue, expectedUsername);
                    if (!validationResult.Ok)
                    {
                        context.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);
                    }
                }
            }
        }


        //public string[] Roles { get; set; }
        //protected override bool IsAuthorized(HttpActionContext actionContext)
        //{
        //    //IPrincipal incomingPrincipal = actionContext.RequestContext.Principal;
        //    //Debug.WriteLine(string.Format("Principal is authenticated at the start of IsAuthorized in CustomAuthorizationFilterAttribute: {0}", incomingPrincipal.Identity.IsAuthenticated));
        //    return false;
        //}

        //public override void OnAuthorization(AuthorizationContext context)
        //{
        //    HttpActionContext ctxt = context.HttpContext as HttpActionContext;

        //    var authorization = context.Request.Headers.Authorization;

        //    if (authorization == null || string.IsNullOrEmpty(authorization.Parameter))
        //    {
        //        // context.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        //        HandleUnauthorizedRequest(context);
        //    }
        //    else
        //    {
        //        var splitToken = authorization.Parameter.Split("bearer ");
        //        var tokenValue = splitToken.Any() ? splitToken[1] : null;
        //        if (string.IsNullOrEmpty(tokenValue))
        //        {
        //            context.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        //        }
        //        else
        //        {
        //            var tokenIssuer = Environment.GetEnvironmentVariable("AUTH_ISSUER");
        //            var tokenAudience = Environment.GetEnvironmentVariable("AUTH_AUDIENCE");
        //            var signingKey = Environment.GetEnvironmentVariable("AUTH_KEY");
        //            var tokenValidation = new Token(tokenIssuer, tokenAudience, signingKey);
        //            var expectedUsername = IsAdminRoleRequired() ? Environment.GetEnvironmentVariable("ADMIN_USERNAME") : null;
        //            var validationResult = tokenValidation.ValidateToken(tokenValue, expectedUsername);
        //            if (!validationResult.Ok)
        //            {
        //                context.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        //            }
        //        }
        //    }
        //}

        private bool IsAdminRoleRequired()
        {
            return Roles.Contains(RequestRole.ADMIN);
        }

        //public bool AllowMultiple => throw new NotImplementedException();

        //public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        //{
        //    var request = context.Request;
        //    var authorization = request.Headers.Authorization;

        //    if (authorization == null || string.IsNullOrEmpty(authorization.Parameter))
        //    {
        //        context.ErrorResult = new StatusCodeResult(System.Net.HttpStatusCode.Unauthorized, request);
        //    }
        //    else
        //    {
        //        var splitToken = authorization.Parameter.Split("bearer ");
        //        var tokenValue = splitToken.Any() ? splitToken[1] : null;
        //        if (string.IsNullOrEmpty(tokenValue))
        //        {
        //            context.ErrorResult = new StatusCodeResult(System.Net.HttpStatusCode.Unauthorized, request);
        //        }
        //        else
        //        {
        //            var tokenValidation = new Token("iss", "aud", "sign");
        //            var validationResult = tokenValidation.ValidateToken(tokenValue);
        //            if (validationResult.Ok)
        //            {
        //                context.Principal = new System.Security.Principal.GenericPrincipal(new Microsoft.AspNetCore.Identity.identit, new string[] { });
        //            }
        //        }
        //    }
        //}

        //public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
