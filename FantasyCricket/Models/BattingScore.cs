using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyCricket.Models
{
    public class BattingScore
    {
        [JsonProperty("pid")]
        public int Pid { get; set; }

        [JsonProperty("R")]
        public int Runs { get; set; }

        [JsonProperty("B")]
        public int Balls { get; set; }

        [JsonProperty("4s")]
        public int Fours { get; set; }

        [JsonProperty("6s")]
        public int Sixes { get; set; }

        [JsonProperty("dismissal")]
        public string Dismissal { get; set; }

    }
}
