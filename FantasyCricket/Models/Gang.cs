using Sqlite.SqliteAttributes;

namespace FantasyCricket.Models
{
    public class Gang
    {

        [SQLiteColumn("name")]
        public int GangName { get; set; }

        [SQLiteColumn("owner")]
        public string GangOwner { get; set; }

        [SQLiteColumn("seriesname")]
        public string SeriesName { get; set; }

    }
}
