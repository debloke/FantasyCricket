using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FantasyCricket.Models
{
    public enum MatchType
    {
        ODI,

    }
    public class PlayerScoresResponse
    {

        [JsonProperty("data")]
        public PlayerScores Data { get; set; }

        [JsonProperty("type")]
        public MatchType MatchType { get; set; }

    }
}
