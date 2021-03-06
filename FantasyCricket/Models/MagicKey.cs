﻿using Newtonsoft.Json;
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

        [SQLiteColumn("username")]
        public string UserName { get; set; }

        [SQLiteColumn("displayname")]
        public string DisplayName { get; set; }

        [SQLiteColumn("password")]
        [JsonIgnore]
        public string Password { get; set; }



    }
}
