using Sqlite.SqliteAttributes;

namespace FantasyCricket.Models
{
    public class Gang
    {

        [SQLiteColumn("name")]
        public string GangName { get; set; }

        [SQLiteColumn("owner")]
        public string GangOwner { get; set; }

        [SQLiteColumn("seriesid")]
        public int SeriesId { get; set; }

        [SQLiteColumn("gangid")]
        public int GangId { get; set; }

        [SQLiteColumn("username")]
        public string Username { get; set; }

    }
}
