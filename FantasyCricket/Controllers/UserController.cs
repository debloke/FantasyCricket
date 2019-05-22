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
