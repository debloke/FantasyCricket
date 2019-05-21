using FantasyCricket.Models;
using FantasyCricket.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FantasyCricket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly ISeriesInfo seriesInfo;
        public SeriesController(ISeriesInfo seriesInfo)
        {
            this.seriesInfo = seriesInfo;
        }

        // GET api/series
        [HttpGet]
        [Route("matches")]
        public ActionResult<IEnumerable<Match>> GetSeriesMatches()
        {
            return seriesInfo.GetMatches();
        }

        // GET api/series
        [HttpGet]
        public ActionResult<IEnumerable<Series>> GetSeries()
        {
            return seriesInfo.GetSeries();
        }

        // GET api/series
        [HttpGet]
        [Route("unassigned")]
        public ActionResult<IEnumerable<Match>> GetUnassigned()
        {
            return seriesInfo.GetUnAssignedMatches();
        }

        // POST api/series
        [HttpPost("{seriesname}")]
        public void CreateSeries(string seriesName)
        {
            seriesInfo.CreateSeries(seriesName);
        }

        // POST api/series
        [HttpPost]
        public void CreateMatch([FromBody] Match match)
        {
            seriesInfo.AddMatch(match);
        }



    }
}
