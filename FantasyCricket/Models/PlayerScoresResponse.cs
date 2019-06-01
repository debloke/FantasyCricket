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

        [JsonProperty("data",Required = Required.Always)]
        
        public PlayerScores Data { get; set; }

        [JsonProperty("type")]
        public MatchType MatchType { get; set; }

    }
}
