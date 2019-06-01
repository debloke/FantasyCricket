using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FantasyCricket.Models
{
    public class Matches
    {

        [JsonProperty("Matches", Required = Required.Always)]
        public Match[] AllMatch { get; set; }

    }
}
