using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sqlite.Converters;
using Sqlite.SqliteAttributes;

namespace FantasyCricket.Models
{
    public enum Role
    {
        BAT,
        BOWL,
        WK,
        ALL,
        UNKNOWN
    }
    public class Player
    {

        [JsonProperty("pid")]
        [SQLiteColumn("Pid")]
        public int PlayerId { get; set; }


        [JsonProperty("name")]
        [SQLiteColumn("Pname")]
        public string PlayerName { get; set; }

        [SQLiteColumn("Cost")]
        public int Cost { get; set; }

        [SQLiteColumn("Team")]
        public string TeamName { get; set; }

        [SQLiteColumn("Role", ConverterType = typeof(SqlStringEnumConverter))]
        [JsonConverter(typeof(StringEnumConverter))]
        public Role Role { get; set; }

    }
}
