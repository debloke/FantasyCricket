using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyCricket.Models
{
    public class BowlingScore
    {
        [JsonProperty("pid")]
        public int Pid { get; set; }

        [JsonProperty("O")]
        public float Overs { get; set; }

        [JsonProperty("M")]
        public int Maidens { get; set; }

        [JsonProperty("R")]
        public int Runs { get; set; }

        [JsonProperty("W")]
        public int Wickets { get; set; }

        [JsonProperty("0s")]
        public int DotBalls { get; set; }

    }
}
