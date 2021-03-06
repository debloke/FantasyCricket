﻿using FantasyCricket.Database;
using FantasyCricket.KeyManager;
using FantasyCricket.Models;
using FantasyCricket.ScoreCalculator;
using FantasyCricket.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Sqlite.SqlClient;
using System;
using System.Collections.Concurrent;
using System.Data.SQLite;
using System.Linq;
using System.Threading;

namespace FantasyCricket.Service
{
    public class CricApiLiveScore : ILiveScore
    {
        private static readonly string SQLSELECT_SCHEDULED_OR_STARTED_MATCHES = "SELECT * FROM [Match] WHERE (STATUS = 0 OR Status = 1) AND MatchTime < datetime('now')";

        private static readonly string SQLSETMATCHSTARTED = "UPDATE [Match] SET Status =@status WHERE unique_id =@unique_id";

        private static readonly string UPDATEUSERLASTTEAM = "update UserTeam set lastteam = currentteam,lastremsub=remsub";

        private static readonly string UPDATEUSERTEAMMATCHMAP = "INSERT OR REPLACE INTO UserTeamPointsHistory (username, selectedteam,unique_id) SELECT username, lastteam,@unique_id FROM UserTeam";


        private static readonly string GETALLUSERMATCHMAP = "SELECT username,selectedteam FROM UserTeamPointsHistory where unique_id=@unique_id";

        private static readonly string UPDATEUSERMATCHMAP = "UPDATE UserTeamPointsHistory SET points=@points where username=@username and unique_id=@unique_id";

        private static readonly string UPDATEMATCHPOINTSHISTORY = "INSERT OR REPLACE INTO MatchPointsHistory (unique_id,points) VALUES (@unique_id, @points)";

        private static readonly string GETMATCHPOINTSHISTORY = "Select points FROM MatchPointsHistory WHERE unique_id = @unique_id";

        private static readonly string GETALLUSERSCORE = "SELECT u.displayname,u.username,SUM(p.points) FROM User u, UserTeamPointsHistory p WHERE u.username=p.username GROUP BY u.username";

        private static readonly string GETALLUSERSCOREINGANG = "SELECT g.username,SUM(p.points) FROM GangUserMap g, UserTeamPointsHistory p WHERE g.username=p.username AND g.name=@gang GROUP BY g.username;";

        private IHubContext<LiveScoreHub> hubContext;

        private ConcurrentDictionary<int, Points[]> liveScores = new ConcurrentDictionary<int, Points[]>();

        private Timer LiveScoreCheckTimer { get; set; }

        private const int LiveScoreCheckTimerPeriod = 300000;  // once every 5 minutes

        private const int TimerStartImmediate = 0;

        public CricApiLiveScore(IHubContext<LiveScoreHub> hubContext)
        {
            this.hubContext = hubContext;
#if (!DEBUG)


            LoadCricApiLiveScoreTimer();
#endif
        }

        private Points[] GetScore(Match match)
        {
            Points[] points = null;

            try
            {
                PlayerScoresResponse playerScoresResponse = new RestExecutor<PlayerScoresResponse>().Invoke(string.Format("https://cricapi.com/api/fantasySummary?unique_id={0}&", match.MatchId));


                if (match.Status != MatchStatus.STARTED && playerScoresResponse.Data.MatchStarted)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
                    {
                        connection.Open();
                        using (SQLiteCommand insertCommand = new SQLiteCommand(SQLSETMATCHSTARTED, connection))
                        {
                            insertCommand.CommandType = System.Data.CommandType.Text;
                            insertCommand.Parameters.AddWithValue("@status", MatchStatus.STARTED);
                            insertCommand.Parameters.AddWithValue("@unique_id", match.MatchId);
                            insertCommand.ExecuteNonQuery();
                        }

                        // update user last team
                        using (SQLiteCommand insertCommand = new SQLiteCommand(UPDATEUSERLASTTEAM, connection))
                        {
                            insertCommand.CommandType = System.Data.CommandType.Text;
                            insertCommand.ExecuteNonQuery();
                        }

                        using (SQLiteCommand insertCommand = new SQLiteCommand(UPDATEUSERTEAMMATCHMAP, connection))
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



                if (playerScoresResponse.Data.MOM != null)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
                    {
                        connection.Open();
                        using (SQLiteCommand insertCommand = new SQLiteCommand(SQLSETMATCHSTARTED, connection))
                        {
                            insertCommand.CommandType = System.Data.CommandType.Text;
                            insertCommand.Parameters.AddWithValue("@status", MatchStatus.FINISHED);
                            insertCommand.Parameters.AddWithValue("@unique_id", match.MatchId);
                            insertCommand.ExecuteNonQuery();
                        }


                        using (SQLiteCommand insertCommand = new SQLiteCommand(GETALLUSERMATCHMAP, connection))
                        {
                            insertCommand.CommandType = System.Data.CommandType.Text;
                            insertCommand.Parameters.AddWithValue("@unique_id", match.MatchId);
                            using (SQLiteDataReader reader = insertCommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        // Get Column ordinal
                                        int ordinal = reader.GetOrdinal("selectedteam");

                                        UserTeam selectedTeam = JsonConvert.DeserializeObject<UserTeam>(reader.GetString(ordinal));


                                        int userOrdinal = reader.GetOrdinal("username");

                                        string username = reader.GetString(userOrdinal);



                                        //calculate points

                                        using (SQLiteCommand updateuserpointmap = new SQLiteCommand(UPDATEUSERMATCHMAP, connection))
                                        {
                                            updateuserpointmap.CommandType = System.Data.CommandType.Text;
                                            updateuserpointmap.Parameters.AddWithValue("@points", PointsCalculator.CalculatePoints(points, selectedTeam));
                                            updateuserpointmap.Parameters.AddWithValue("@unique_id", match.MatchId);
                                            updateuserpointmap.Parameters.AddWithValue("@username", username);
                                            updateuserpointmap.ExecuteNonQuery();
                                        }

                                    }

                                }
                            }
                        }


                        using (SQLiteCommand insertCommand = new SQLiteCommand(UPDATEMATCHPOINTSHISTORY, connection))
                        {
                            insertCommand.CommandType = System.Data.CommandType.Text;
                            insertCommand.Parameters.AddWithValue("@points", JsonConvert.SerializeObject(points));
                            insertCommand.Parameters.AddWithValue("@unique_id", match.MatchId);
                            insertCommand.ExecuteNonQuery();
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                // TODO
            }
            return points;
        }

        public void LoadCricApiLiveScoreTimer()
        {
            try
            {
                LiveScoreCheckTimer = null;   // Ensure null initialization

                LiveScoreCheckTimer = new Timer(new TimerCallback(LiveScoreCheckTimerEvent), this, TimerStartImmediate, LiveScoreCheckTimerPeriod);
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

        public void LiveScoreCheckTimerEvent(object state)
        {


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

            if (liveMatches.Length > 0)
            {
                liveScores = new ConcurrentDictionary<int, Points[]>();
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

            hubContext.Clients.All.SendAsync("ReceiveLiveScores", JsonConvert.SerializeObject(liveScores, Formatting.Indented));



        }

        public ConcurrentDictionary<int, Points[]> GetScores()
        {
            return liveScores;
        }

        public UserPoints[] GetUserPoints()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(GETALLUSERSCORE, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        return reader.ReadAll<UserPoints>();
                    }
                }
            }
        }

        public UserPoints[] GetUserPoints(string gang)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(GETALLUSERSCOREINGANG, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@gang", gang);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        return reader.ReadAll<UserPoints>();
                    }
                }
            }
        }


        public void UpdateMatchPointsHistory(int unique_id)
        {
            PlayerScoresResponse playerScoresResponse = new RestExecutor<PlayerScoresResponse>().Invoke(string.Format("https://cricapi.com/api/fantasySummary?unique_id={0}&", unique_id));

            Points[] points = null;
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

            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand insertCommand = new SQLiteCommand(UPDATEMATCHPOINTSHISTORY, connection))
                {
                    insertCommand.CommandType = System.Data.CommandType.Text;
                    insertCommand.Parameters.AddWithValue("@points", JsonConvert.SerializeObject(points));
                    insertCommand.Parameters.AddWithValue("@unique_id", unique_id);
                    insertCommand.ExecuteNonQuery();
                }

            }


        }


        public Points[] GetMatchPointsHistory(int unique_id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand insertCommand = new SQLiteCommand(GETMATCHPOINTSHISTORY, connection))
                {
                    insertCommand.CommandType = System.Data.CommandType.Text;
                    insertCommand.Parameters.AddWithValue("@unique_id", unique_id);
                    using (SQLiteDataReader reader = insertCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // Get Column ordinal
                                int ordinal = reader.GetOrdinal("points");

                                return JsonConvert.DeserializeObject<Points[]>(reader.GetString(ordinal));
                            }
                        }
                        return null;
                    }
                }
            }
        }
    }
}

