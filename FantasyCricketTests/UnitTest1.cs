using FantasyCricket.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace FantasyCricketTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void NoOFBallsTest()
        {
            float overs = 9.3f;
            int balls = 0;
            balls += (int)(overs / 1) * 6;
            balls+=(int)((overs % 1.0f)*10.0);
            Console.WriteLine(balls);


        }

        [TestMethod]
        public void TestMethodMatches()
        {
            string matchJson = File.ReadAllText(@"Match.json", Encoding.UTF8);

            Matches matches = JsonConvert.DeserializeObject<Matches>(matchJson);

            Console.WriteLine(matches);
        }



        [TestMethod]
        public void TestMethod1()
        {
            string fantasySummaryJson = File.ReadAllText(@"FantasySummary.json", Encoding.UTF8);

            PlayerScoresResponse playerScoresResponse = JsonConvert.DeserializeObject<PlayerScoresResponse>(fantasySummaryJson, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });



            ConcurrentDictionary<int, Points> concurrentDictionary = new ConcurrentDictionary<int, Points>();

            foreach (Team team in playerScoresResponse.Data.Team)
            {
                foreach (Player player in team.Players)
                {
                    concurrentDictionary.TryAdd(player.PlayerId, new Points() { PlayerId = player.PlayerId, PlayerName = player.PlayerName });
                }
            }

            foreach (Batting battingDetails in playerScoresResponse.Data.Batting)
            {
                foreach (BattingScore battingScore in battingDetails.BattingScores)
                {
                    if (battingScore.Pid != 0)
                    {
                        concurrentDictionary.TryGetValue(battingScore.Pid, out Points points);
                        points.BattingScore = battingScore;
                    }
                }
            }

            foreach (Bowling bowlingDetails in playerScoresResponse.Data.Bowling)
            {
                foreach (BowlingScore bowlingScore in bowlingDetails.BowlingScores)
                {
                    concurrentDictionary.TryGetValue(bowlingScore.Pid, out Points points);
                    points.BowlingScore = bowlingScore;
                }
            }

            foreach (Fielding fieldingDetails in playerScoresResponse.Data.Fielding)
            {
                foreach (FieldingScore fieldingScore in fieldingDetails.FieldingScores)
                {
                    concurrentDictionary.TryGetValue(fieldingScore.Pid, out Points points);
                    points.FieldingScore = fieldingScore;
                }
            }




            foreach (Points point in concurrentDictionary.Values)
            {
                Console.WriteLine(JsonConvert.SerializeObject(point));
            }





        }
    }
}
