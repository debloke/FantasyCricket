using Newtonsoft.Json;
using Sqlite.SqliteAttributes;

namespace FantasyCricket.Models
{
    public class Gang
    {


        [SQLiteColumn("name")]
        public string Name { get; set; }
        public bool Owner { get; set; }

        [JsonIgnore]
        [SQLiteColumn("username")]
        public string Username { get; set; }




    }
}
