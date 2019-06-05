using FantasyCricket.Models;
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
        public ActionResult<string> Get() => JsonConvert.SerializeObject(liveScore.GetScores(), Formatting.Indented); 
          
            

        // GET api/score
        [HttpGet]
        [Route("test")]
        public ActionResult<string> GetTest()
        {
            liveScore.LiveScoreCheckTimerEvent(new object());
            return JsonConvert.SerializeObject(liveScore.GetScores(), Formatting.Indented);
        }


        // GET api/score
        [HttpGet]
        [Route("points")]
        public ActionResult<UserPoints[]> GetUserPoints()
        {
            return liveScore.GetUserPoints();
        }


        [HttpGet]
        [Route("points/{gang}")]
        public ActionResult<UserPoints[]> GetUserPointsInGang(string gang)
        {
            return liveScore.GetUserPoints(gang);
        }


    }
}
