﻿using FantasyCricket.Models;
using FantasyCricket.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FantasyCricket.Controllers
{
    [Authorize(AuthenticationSchemes = "MagickeyAuthentication")]
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

        [HttpDelete]
        [Route("matches/{uniqueid}")]
        public void DeleteSeriesMatches(int uniqueid)
        {
            seriesInfo.DeleteMatch(uniqueid);
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

        [HttpPost("cancel/{id}")]
        public void CancelMatch(int id)
        {
            seriesInfo.CancelMatch(id);
        }


    }
}
