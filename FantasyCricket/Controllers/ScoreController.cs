using FantasyCricket.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        [HttpGet]
        public ActionResult<string> Get() => JsonConvert.SerializeObject(liveScore.GetScores(),Formatting.Indented);
    }
}
