using Common.Net.Extensions;
using FantasyCricket.Database;
using FantasyCricket.Models;
using Newtonsoft.Json;
using Sqlite.SqlClient;
using System.Data.SQLite;
using System.Net.Http;

namespace FantasyCricket.Service
{
    public class SeriesInfo : ISeriesInfo
    {
        private readonly HttpClient httpClient = new HttpClient();

        private static readonly string SQLSELECTMATCH = "SELECT * FROM Match";


        private static readonly string SQLSELECTSERIES = "SELECT * FROM Series";

        private static readonly string CREATESERIES = "INSERT OR REPLACE INTO [Series] (  Seriesname ) VALUES (  @Seriesname)";
        private static readonly string ADDORUPDATEMATCH = "INSERT OR REPLACE INTO [Match] (  uniqie_id, MatchTime, Seriesid, Type,team1,team2 ) VALUES ( @uniqie_id, @MatchTime, @Seriesid, @Type,@team1,@team2)";

        public void AddMatch(Match match)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand insertCommand = new SQLiteCommand(ADDORUPDATEMATCH, connection))
                {
                    insertCommand.CommandType = System.Data.CommandType.Text;
                    insertCommand.Parameters.AddWithValue("@uniqie_id", match.MatchId);
                    insertCommand.Parameters.AddWithValue("@MatchTime", match.MatchStartTime);
                    insertCommand.Parameters.AddWithValue("@Seriesid", match.SeriesId);
                    insertCommand.Parameters.AddWithValue("@Type", match.Type);
                    insertCommand.Parameters.AddWithValue("@team1", match.Team1);
                    insertCommand.Parameters.AddWithValue("@team2", match.Team2);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        public void CreateSeries(string seriesName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand insertCommand = new SQLiteCommand(CREATESERIES, connection))
                {
                    insertCommand.CommandType = System.Data.CommandType.Text;
                    insertCommand.Parameters.AddWithValue("@Seriesname", seriesName);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        public Match[] GetMatches()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(SQLSELECTMATCH, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        return reader.ReadAll<Match>();
                    }
                }
            }
        }

        public Match[] GetUnAssignedMatches()
        {
            Matches matches = JsonConvert.DeserializeObject<Matches>(httpClient.InvokeGet(string.Format("https://cricapi.com/api/matches?apikey=ZlrRCAEEwjg9Vknh9hOgVlV17ls2")));
            return matches.AllMatch;
        }

        public Series[] GetSeries()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(SQLSELECTSERIES, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        return reader.ReadAll<Series>();
                    }
                }
            }
        }
    }
}
