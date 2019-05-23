using FantasyCricket.Service;
using Newtonsoft.Json;

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

        [JsonProperty("winner_team")]
        public CountryTeamName WinnerTeam { get; set; }

        [JsonProperty("matchStarted")]
        public bool MatchStarted { get; set; }

        [JsonProperty("man-of-the-match")]
        public Player MOM { get; set; }
    }
}
