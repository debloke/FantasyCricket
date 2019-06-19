using FantasyCricket.Models;
using FantasyCricket.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FantasyCricket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "MagickeyAuthentication")]
    public class GangsController : ControllerBase
    {
        private readonly IGangs gangs;
        private readonly IHttpContextAccessor httpContextAccessor;

        public GangsController(IGangs gangs, IHttpContextAccessor httpContextAccessor)
        {
            this.gangs = gangs;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public ActionResult<Gang[]> GetGangs([FromQuery(Name = "seriesid")]int seriesid)

        {
            return gangs.GetGangs(httpContextAccessor.HttpContext.User.Identity.Name, seriesid);
        }

        [HttpPost]
        public void CreateGang([FromBody] Gang gang)

        {

            gang.GangOwner = httpContextAccessor.HttpContext.User.Identity.Name;
            gangs.CreateGang(gang);
        }

        [HttpPost("{gangid}")]
        public void AddToGang(int gangid, [FromBody] string[] usernames)

        {

            gangs.AddToGang(gangid, usernames, httpContextAccessor.HttpContext.User.Identity.Name);
        }


    }
}
