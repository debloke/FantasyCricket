using Newtonsoft.Json;
using Sqlite.SqliteAttributes;

namespace FantasyCricket.Models
{
    public class UserTeamHistory
    {

        [SQLiteColumn("unique_id")]
        public int MatchId { get; set; }


        public UserTeam SelectedTeam
        {
            get
            {
                return JsonConvert.DeserializeObject<UserTeam>(this.SelectedTeamString);
            }
            set { }
        }


        [SQLiteColumn("selectedteam")]
        [JsonIgnore]
        public string SelectedTeamString { get; set; }

        [SQLiteColumn("points")]
        public int Points { get; set; }


    }
}
