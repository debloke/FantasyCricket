using Sqlite.SqliteAttributes;
using System;

namespace FantasyCricket.Models
{
    public class UserPoints
    {

        [SQLiteColumn("username")]
        public string Username { get; set; }

        [SQLiteColumn("SUM(p.points)")]
        public Int64 Total { get; set; }


    }
}
