using System;
using System.Linq;
using System.Security.Claims;
using Helpo.Common;

namespace Helpo.Services.Auth
{
    public class LoggedInUser
    {
        private readonly ClaimsPrincipal _claimsPrincipal;
        
        public string UserId { get; }

        public LoggedInUser(ClaimsPrincipal claimsPrincipal)
        {
            Guard.NotNull(claimsPrincipal, nameof(claimsPrincipal));
            
            this._claimsPrincipal = claimsPrincipal;

            this.UserId = this._claimsPrincipal.Claims?.FirstOrDefault(f => f.Type == "HelpoId")?.Value ?? throw new ArgumentException("Invalid claims identity.");
        }
    }
}