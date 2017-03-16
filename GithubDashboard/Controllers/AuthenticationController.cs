using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using GithubDashboard.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public static class HttpContextExtensions
    {
        public static IEnumerable<AuthenticationDescription> GetExternalProviders(this HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return from description in context.Authentication.GetAuthenticationSchemes()
                   where !string.IsNullOrWhiteSpace(description.DisplayName)
                   select description;
        }

        public static bool IsProviderSupported(this HttpContext context, string provider)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return (from description in context.GetExternalProviders()
                    where string.Equals(description.AuthenticationScheme, provider, StringComparison.OrdinalIgnoreCase)
                    select description).Any();
        }
    }

    public static class UserExtensions
    {
        public static string GetEmail(this ClaimsPrincipal user)
        {
            // Yes yes i know :D this code sux
            return user.Claims.SingleOrDefault(x => x.Value.Contains("@")).Value;
        }
    }

    public class AuthenticationController : Controller
    {
        [HttpGet("~/signin")]
        public IActionResult SignIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("SignIn", HttpContext.GetExternalProviders());
        }

        [HttpPost("~/signin")]
        public IActionResult SignIn([FromForm] string provider, [FromForm] string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest();
            }

            if (!HttpContext.IsProviderSupported(provider))
            {
                return BadRequest();
            }

            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, provider);
        }

        [HttpGet("~/signout"), HttpPost("~/signout")]
        public IActionResult SignOut()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
