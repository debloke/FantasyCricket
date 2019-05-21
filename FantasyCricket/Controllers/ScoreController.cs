using FantasyCricket.Models;
using FantasyCricket.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FantasyCricket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private readonly ILiveScore liveScore;
        public ScoreController(ILiveScore liveScore)
        {
            this.liveScore = liveScore;
        }

        // GET api/score
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Points>> Get(int id) => liveScore.GetScore(id);
    }
}
