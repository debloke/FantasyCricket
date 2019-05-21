using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FantasyCricket.Models
{
    public class Matches
    {

        [JsonProperty("Matches")]
        public Match[] AllMatch { get; set; }

    }
}
