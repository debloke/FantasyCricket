using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FantasyCricket.Models
{
    public class PlayerScores
    {

        [JsonProperty("fielding")]
        public Fielding[] Fielding { get; set; }

        [JsonProperty("batting")]
        public Batting[] Batting { get; set; }

        [JsonProperty("bowling")]
        public Bowling[] Bowling { get; set; }

        [JsonProperty("team")]
        public Team[] Team { get; set; }
    }
}
