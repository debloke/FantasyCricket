using FantasyCricket.Models;
using FantasyCricket.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace FantasyCricket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GangsController : ControllerBase
    {
        private readonly IGangs gangs;
        public GangsController(IGangs gangs)
        {
            this.gangs = gangs;
        }

        [HttpGet]
        public ActionResult<Gang[]> GetGangs([FromQuery(Name = "magic")] Guid magicKey)

        {
            return gangs.GetGangs(magicKey);
        }
    }
}
