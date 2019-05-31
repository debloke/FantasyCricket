using FantasyCricket.Converter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyCricket.Models
{
    public class Team
    {

        [JsonProperty("players")]
        public Player[] Players { get; set; }


        [JsonProperty("name")]
        public string TeamName { get; set; }


    }
}
