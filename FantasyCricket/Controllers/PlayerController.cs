using FantasyCricket.Models;
using FantasyCricket.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace FantasyCricket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerInfo playerInfo;
        public PlayerController(IPlayerInfo playerInfo)
        {
            this.playerInfo = playerInfo;
        }

        // GET api/player
        [HttpGet]
        public ActionResult<IEnumerable<Player>> Get([FromQuery(Name = "tournament")] TeamType teamType)
        {
            return playerInfo.GetPlayers(teamType);
        }


        // PUT api/player/{matchId}
        // Update Player Data base with Player Info         
        [HttpPut("{id}")]
        public void Put(int id, [FromQuery(Name = "tournament")] TeamType teamType)
        {
            switch (teamType)
            {

                case TeamType.IPL:
                    playerInfo.UpdateIplPlayers(id);
                    break;
                case TeamType.ODI:
                    playerInfo.UpdateCountryPlayers(id);
                    break;
                default:
                    throw new Exception("Unsupported tournament");
            }
        }


        [HttpDelete("{id}")]
        public void Delete(int id, [FromQuery(Name = "tournament")] TeamType teamType)
        {
            playerInfo.DeletePlayer(id, teamType);
        }


        // PUT api/player/{matchId}
        // Update Player Data base with Player Info         
        [HttpPut("{id}/{cost}/{role}")]
        public void Put(int id, int cost, Role role, [FromQuery(Name = "tournament")] TeamType teamType)
        {
            playerInfo.UpdatePlayerCost(id, cost, role, teamType);
        }
    }
}
