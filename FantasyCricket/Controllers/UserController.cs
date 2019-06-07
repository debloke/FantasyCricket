using FantasyCricket.Models;
using FantasyCricket.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FantasyCricket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser user;

        private readonly IHttpContextAccessor httpContextAccessor;

        public UserController(IUser user, IHttpContextAccessor httpContextAccessor)
        {
            this.user = user;
            this.httpContextAccessor = httpContextAccessor;
        }

        // POST api/user
        [Authorize(AuthenticationSchemes ="BasicAuthentication")]
        [HttpPost]
        public ActionResult<MagicKey> LoginUser()
        {
            
            return user.LoginUser(httpContextAccessor.HttpContext.User.Identity.Name);
        }

        [Authorize(AuthenticationSchemes = "MagickeyAuthentication")]
        [HttpPost("logout")]
        public void LogoutUser()
        {

            user.LogoutUser(httpContextAccessor.HttpContext.User.Identity.Name);
        }

        [Authorize(AuthenticationSchemes = "MagickeyAuthentication")]
        [HttpPost("renew")]
        public ActionResult<MagicKey> RenewUser()
        {

            return user.LoginUser(httpContextAccessor.HttpContext.User.Identity.Name);
        }


        // POST api/user
        [Authorize(AuthenticationSchemes = "MagickeyAuthentication")]
        [HttpPost]
        [Route("team")]
        public void SaveTeam(UserTeam userTeam)

        {
            user.SaveTeam(userTeam, httpContextAccessor.HttpContext.User.Identity.Name);
        }

        [HttpGet]
        [Route("team")]
        [Authorize(AuthenticationSchemes = "MagickeyAuthentication")]
        public UserTeam GetTeam()

        {
            return user.GetTeam(httpContextAccessor.HttpContext.User.Identity.Name);
        }

        [Authorize(AuthenticationSchemes = "MagickeyAuthentication")]
        [HttpGet]
        [Route("team/others")]
        public UserTeam GetLastTeam([FromQuery(Name = "username")] string username)

        {
            return user.GetLastTeam(username);
        }

        [Authorize(AuthenticationSchemes = "MagickeyAuthentication")]
        [HttpGet]
        [Route("team/history")]
        public UserTeamHistory[] GetHistoryTeam([FromQuery(Name = "username")] string username)

        {
            return user.GetHistoryTeam(username);
        }

        [Authorize(AuthenticationSchemes = "MagickeyAuthentication")]
        [HttpGet]
        public ActionResult<string[]> GetUsers()

        {
            return user.GetUsers();
        }

        // PUT api/user
        [HttpPut]
        public void CreateUser(UserRegistration userRegistration)
        {
            user.CreateUser(userRegistration.UserName, userRegistration.Password, userRegistration.DisplayName);
        }

    }
}
