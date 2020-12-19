using System;
using System.Linq;
using System.Security.Claims;
using Helpo.Common;

namespace Helpo.Auth
{
    public class LoggedInUser
    {
        public string UserId { get; }
        
        public ClaimsPrincipal Principal { get; }

        public LoggedInUser(ClaimsPrincipal claimsPrincipal)
        {
            Guard.NotNull(claimsPrincipal, nameof(claimsPrincipal));
            
            this.Principal = claimsPrincipal;

            this.UserId = this.Principal.Claims?.FirstOrDefault(f => f.Type == "HelpoId")?.Value ?? throw new ArgumentException("Invalid claims identity.");
        }
    }
}