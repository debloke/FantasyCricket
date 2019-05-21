using FantasyCricket.Converter;
using FantasyCricket.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sqlite.Converters;
using Sqlite.SqliteAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyCricket.Models
{
    public class Match
    {

        [JsonProperty("unique_id")]
        [SQLiteColumn("unique_id")]
        public int MatchId { get; set; }


        [JsonProperty("dateTimeGMT")]
        [SQLiteColumn("MatchTime")]
        public DateTime MatchStartTime { get; set; }

        [SQLiteColumn("SeriesId")]
        public int SeriesId { get; set; }

        [SQLiteColumn("Type")]
        [JsonProperty("type")]
        public string Type { get; set; }

        [SQLiteColumn("team1",ConverterType=typeof(SqlStringEnumConverter))]
        [JsonConverter(typeof(CountryNameJsonConverter))]
        public CountryTeamName Team1 { get; set; }

        [SQLiteColumn("team2", ConverterType = typeof(SqlStringEnumConverter))]
        [JsonConverter(typeof(CountryNameJsonConverter))]
        public CountryTeamName Team2 { get; set; }

    }
}
