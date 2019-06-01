using FantasyCricket.Models;
using FantasyCricket.Service;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FantasyCricket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser user;
        public UserController(IUser user)
        {
            this.user = user;
        }

        // POST api/user
        [HttpPost]
        public ActionResult<Guid> LoginUser([FromQuery(Name = "username")] string username,
            [FromQuery(Name = "password")] string password)

        {
            return user.LoginUser(username, password).Magic;
        }

        // POST api/user
        [HttpPost]
        [Route("team")]
        public void SaveTeam(UserTeam userTeam,
            [FromQuery(Name = "magic")] Guid magicKey)

        {
            user.SaveTeam(userTeam, magicKey);
        }


        [HttpGet]
        [Route("team")]
        public UserTeam GetTeam([FromQuery(Name = "magic")] Guid magicKey)

        {
            return user.GetTeam(magicKey);
        }

        [HttpGet]
        [Route("team/others")]
        public UserTeam GetLastTeam([FromQuery(Name = "username")] string username)

        {
            return user.GetLastTeam(username);
        }

        [HttpGet]
        public ActionResult<string[]> GetUsers()

        {
            return user.GetUsers();
        }

        // PUT api/user
        [HttpPut]
        public void CreateUser([FromQuery(Name = "username")] string username,
            [FromQuery(Name = "password")] string password,
            [FromQuery(Name = "displayname")] string displayName)
        {
            user.CreateUser(username, password, displayName);
        }

    }
}
