using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Helpo.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet("signin")]
        public async Task<IActionResult> SignIn([FromQuery]string ticket)
        {
            //TODO: Take the ticket and call the c-entron Web-Service to get information abour ourselfes
            
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, "Jürgen"));
            identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            identity.AddClaim(new Claim(ClaimTypes.Role, "Support"));
            
            var principal = new ClaimsPrincipal(identity);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Redirect("/");
        }
        
        [HttpGet("signout")]
        public new IActionResult SignOut()
        {
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = "/",
            }, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}