using FantasyCricket.Database;
using FantasyCricket.KeyManager;
using FantasyCricket.Models;
using Sqlite.SqlClient;
using System.Data.SQLite;

namespace FantasyCricket.Service
{
    public class SeriesInfo : ISeriesInfo
    {
        private static readonly string SQLSELECTMATCH = "SELECT * FROM Match";


        private static readonly string SQLSELECTSERIES = "SELECT * FROM Series";

        private static readonly string CREATESERIES = "INSERT OR REPLACE INTO [Series] (  Seriesname ) VALUES (  @Seriesname)";
        private static readonly string ADDORUPDATEMATCH = "INSERT OR REPLACE INTO [Match] (  unique_id, MatchTime, Seriesid, Type,team1,team2 ) VALUES ( @unique_id, @MatchTime, @Seriesid, @Type,@team1,@team2)";


        private static readonly string DELETEMATCH = "DELETE FROM [Match] where unique_id=@unique_id";

        private static readonly string CANCELMATCH = "UPDATE [Match] SET Status=3 where unique_id=@unique_id";

        public void AddMatch(Match match)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand insertCommand = new SQLiteCommand(ADDORUPDATEMATCH, connection))
                {
                    insertCommand.CommandType = System.Data.CommandType.Text;
                    insertCommand.Parameters.AddWithValue("@unique_id", match.MatchId);
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

        public void CancelMatch(int uniqueid)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand insertCommand = new SQLiteCommand(CANCELMATCH, connection))
                {
                    insertCommand.CommandType = System.Data.CommandType.Text;
                    insertCommand.Parameters.AddWithValue("@unique_id", uniqueid);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        public void DeleteMatch(int uniqueid)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand insertCommand = new SQLiteCommand(DELETEMATCH, connection))
                {
                    insertCommand.CommandType = System.Data.CommandType.Text;
                    insertCommand.Parameters.AddWithValue("@unique_id", uniqueid);
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

            Matches matches = new RestExecutor<Matches>().Invoke(string.Format("https://cricapi.com/api/matches?"));
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
