using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FantasyCricket.Models;
using FantasyCricket.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace FantasyCricket.Authentication
{
    public class MagickeyAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUser userService;

        public MagickeyAuthentication(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUser userService)
            : base(options, logger, encoder, clock)
        {
            this.userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Magic"))
                return AuthenticateResult.Fail("Missing Magic");

            MagicKey user = null;
            try
            {
                user = userService.LoginUser(Guid.Parse(Request.Headers["Magic"]));
            }
            catch(Exception exception)
            {
                return AuthenticateResult.Fail("Invalid Magic");
            }

            if (user == null)
                return AuthenticateResult.Fail("Invalid Magic");

            var claims = new[] {
                new Claim(ClaimTypes.Name, user.username),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}