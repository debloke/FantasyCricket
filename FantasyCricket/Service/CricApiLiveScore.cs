using Common.Net.Extensions;
using FantasyCricket.Database;
using FantasyCricket.Models;
using FantasyCricket.ScoreCalculator;
using Newtonsoft.Json;
using Sqlite.SqlClient;
using System;
using System.Collections.Concurrent;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace FantasyCricket.Service
{
    public class CricApiLiveScore : ILiveScore
    {
        private readonly HttpClient httpClient = new HttpClient();

        private static readonly string SQLSELECT_SCHEDULED_OR_STARTED_MATCHES = "SELECT * FROM [Match] WHERE (STATUS = 0 OR Status = 1) AMS MatchTime < date('now')";

        private static readonly string SQLSETMATCHSTARTED = "UPDATE [Match] SET Status = 1 WHERE unique_id =@unique_id";

        private ConcurrentDictionary<int, Points[]> liveScores = new ConcurrentDictionary<int, Points[]>();



        private Timer LiveScoreCheckTimer { get; set; }

        private const int LiveScoreCheckTimerPeriod = 300000;  // once every 5 minutes

        public CricApiLiveScore()
        {
            LoadCricApiLiveScoreTimer();
        }


        private Points[] GetScore(Match match)
        {
            Points[] points = null;

            try
            {
                string response = httpClient.InvokeGet(string.Format("https://cricapi.com/api/fantasySummary?apikey=ZlrRCAEEwjg9Vknh9hOgVlV17ls2&unique_id={0}", match.MatchId));
                PlayerScoresResponse playerScoresResponse = JsonConvert.DeserializeObject<PlayerScoresResponse>(response, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });


                if (match.Status != MatchStatus.STARTED && playerScoresResponse.Data.MatchStarted)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
                    {
                        connection.Open();
                        using (SQLiteCommand insertCommand = new SQLiteCommand(SQLSETMATCHSTARTED, connection))
                        {
                            insertCommand.CommandType = System.Data.CommandType.Text;
                            insertCommand.Parameters.AddWithValue("@unique_id", match.MatchId);
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }



                IScoreCalculator scoreCalulator;
                switch (playerScoresResponse.MatchType)
                {
                    case MatchType.ODI:
                        scoreCalulator = new OdiScoreCalculator();
                        points = scoreCalulator.CalculateScore(playerScoresResponse).Values.ToArray();
                        break;
                    default:
                        throw new Exception("Unknown match type");

                }
            }
            catch
            {
                // TODO
            }
            return points;
        }

        public void LoadCricApiLiveScoreTimer()
        {
            try
            {
                LiveScoreCheckTimer = null;   // Ensure null initialization

                LiveScoreCheckTimer = new Timer(new TimerCallback(LiveScoreCheckTimerEvent), this, LiveScoreCheckTimerPeriod, LiveScoreCheckTimerPeriod);
            }
            catch (Exception exception)
            {
                Exception rethrow_Exception = new Exception(
                    string.Format("Failed to initialize the Live Score timer. {0}", exception.Message)
                    );

                // Throw exception to DhcpStartService
                throw rethrow_Exception;
            }
        }

        private void LiveScoreCheckTimerEvent(object state)
        {

            liveScores = new ConcurrentDictionary<int, Points[]>();
            Match[] liveMatches;

            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(SQLSELECT_SCHEDULED_OR_STARTED_MATCHES, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        liveMatches = reader.ReadAll<Match>();
                    }
                }
            }

            foreach (Match match in liveMatches)
            {
                try
                {
                    liveScores.GetOrAdd(match.MatchId, GetScore(match));
                }
                catch
                {
                    // TO DO
                }
            }
        }

        public ConcurrentDictionary<int, Points[]> GetScores()
        {
            return liveScores;
        }
    }
}
