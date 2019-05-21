using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyCricket.Models
{
    public class FieldingScore
    {
        [JsonProperty("pid")]
        public int Pid { get; set; }

        [JsonProperty("catch")]
        public int Catch { get; set; }

        [JsonProperty("stumped")]
        public int Stumped { get; set; }

        [JsonProperty("runout")]
        public int Runout { get; set; }

    }
}
