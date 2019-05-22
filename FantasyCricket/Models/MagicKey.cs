using Sqlite.SqliteAttributes;
using System;

namespace FantasyCricket.Models
{
    public class MagicKey
    
    {

        [SQLiteColumn("magickey")]
        public Guid Magic { get; set; }

        [SQLiteColumn("lastlogin")]
        public DateTime LastLogin { get; set; }



    }
}
