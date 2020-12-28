using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Helpo.Pages.Auth
{
    public class AuthController : Controller
    {
        [HttpGet("signin")]
        public async Task<IActionResult> SignIn([FromQuery]string ticket, [FromServices]AuthService authService)
        {
            var claims = await authService.GetClaims(ticket);
            
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(claims);
            
            var principal = new ClaimsPrincipal(identity);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Redirect("/");
        }
        
        [HttpGet("signout")]
        public new IActionResult SignOut()
        {
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = "/login",
            }, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}