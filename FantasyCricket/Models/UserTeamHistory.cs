using Sqlite.SqliteAttributes;
using System;

namespace FantasyCricket.Models
{
    public class UserTeamHistory
    {

        [SQLiteColumn("unique_id")]
        public int MatchId { get; set; }

        [SQLiteColumn("selectedteam")]
        public UserTeam SelectedTeam { get; set; }

        [SQLiteColumn("points")]
        public Int64 Points { get; set; }


    }
}
