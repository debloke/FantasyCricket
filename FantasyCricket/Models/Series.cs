using Sqlite.SqliteAttributes;

namespace FantasyCricket.Models
{
    public class Series
    {

        [SQLiteColumn("Seriesid")]
        public int SeriesId { get; set; }

        [SQLiteColumn("Seriesname")]
        public string SeriesName { get; set; }


    }
}
