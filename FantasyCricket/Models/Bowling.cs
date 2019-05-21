using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyCricket.Models
{
    public class Bowling
    {
        [JsonProperty("scores")]
        public BowlingScore[] BowlingScores { get; set; }



    }
}
