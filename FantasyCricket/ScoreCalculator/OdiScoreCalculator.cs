using FantasyCricket.Models;
using FantasyCricket.Service;
using System;
using System.Collections.Generic;

namespace FantasyCricket.ScoreCalculator
{
    public class OdiScoreCalculator : IScoreCalculator
    {

        private const float CONSTANT_RUNS_PER_BALL = 1.5f;

        public Dictionary<int, Points> CalculateScore(PlayerScoresResponse playerScoresResponse)
        {
            Dictionary<int, Points> playerScores = new Dictionary<int, Points>();


            foreach (Team team in playerScoresResponse.Data.Team)
            {
                string teamName = team.TeamName.Replace(" ", "");
                // Check  If country

                if (Enum.TryParse(typeof(CountryTeamName), teamName, true, out object countryTeamName))
                {

                    foreach (Player player in team.Players)
                    {
                        playerScores.TryAdd(player.PlayerId, new Points() { PlayerId = player.PlayerId, PlayerName = player.PlayerName, Team = teamName });
                    }
                }
            }

            foreach (Batting battingDetails in playerScoresResponse.Data.Batting)
            {
                foreach (BattingScore battingScore in battingDetails.BattingScores)
                {
                    if (battingScore.Pid != 0)
                    {
                        playerScores.TryGetValue(battingScore.Pid, out Points points);
                        points.BattingScore = battingScore;
                        points.BattingPoints = CalculateBattingPoints(points.BattingScore);
                    }
                }
            }

            foreach (Bowling bowlingDetails in playerScoresResponse.Data.Bowling)
            {
                foreach (BowlingScore bowlingScore in bowlingDetails.BowlingScores)
                {
                    playerScores.TryGetValue(bowlingScore.Pid, out Points points);
                    points.BowlingScore = bowlingScore;
                    points.BowlingPoints = CalculateBowlingPoints(points.BowlingScore);
                }
            }

            foreach (Fielding fieldingDetails in playerScoresResponse.Data.Fielding)
            {
                foreach (FieldingScore fieldingScore in fieldingDetails.FieldingScores)
                {
                    playerScores.TryGetValue(fieldingScore.Pid, out Points points);
                    points.FieldingScore = fieldingScore;
                    points.FieldingPoints = CalculateFieldingPoints(points.FieldingScore);
                }
            }

            if (playerScoresResponse.Data.MOM != null)
            {
                playerScores.TryGetValue(playerScoresResponse.Data.MOM.PlayerId, out Points points);
                points.Bonus = 50;

            }



            return playerScores;
        }

        private int CalculateBattingPoints(BattingScore battingScore)
        {
            int points = 0;
            // 1 point for every run
            points += battingScore.Runs;

            // -10 if duck out
            if ((battingScore.Runs == 0) && !String.IsNullOrEmpty(battingScore.Dismissal))
            {
                points -= 10;
            }


            // If run ab ball or more then double less balls
            int runsDiff = battingScore.Runs - battingScore.Balls;
            if (runsDiff >= 0)
            {
                points += (runsDiff * 2);
            }
            else
            {
                points += (runsDiff);
            }

            //Every 10 run-> + 2 point
            int noOf10s = battingScore.Runs / 10;
            if (noOf10s >= 1)
            {
                points += (noOf10s * 2);
            }
            //Every 25 run-> + 5 point
            int noOf25s = battingScore.Runs / 25;
            if (noOf25s >= 1)
            {
                points += (noOf25s * 5);
            }
            //Every 50 run-> + 10 point
            int noOf50s = battingScore.Runs / 50;
            if (noOf50s >= 1)
            {
                points += (noOf50s * 10);
            }

            //Every 100 run-> + 20 point
            int noOf100s = battingScore.Runs / 100;
            if (noOf100s >= 1)
            {
                points += (noOf100s * 20);
            }

            //Every 125 run-> + 25 point
            int noOf125s = battingScore.Runs / 125;
            if (noOf125s >= 1)
            {
                points += (noOf125s * 25);
            }

            //Every 150 run-> + 30 point
            int noOf150s = battingScore.Runs / 150;
            if (noOf150s >= 1)
            {
                points += (noOf150s * 30);
            }

            //Every 175 run-> + 35 point
            int noOf175s = battingScore.Runs / 175;
            if (noOf175s >= 1)
            {
                points += (noOf175s * 35);
            }

            //Every 200 run-> + 40 point
            int noOf200s = battingScore.Runs / 200;
            if (noOf200s >= 1)
            {
                points += (noOf200s * 40);
            }

            //Every 225 run-> + 45 point
            int noOf225s = battingScore.Runs / 225;
            if (noOf225s >= 1)
            {
                points += (noOf225s * 45);
            }

            //Every 250 run-> + 50 point
            int noOf250s = battingScore.Runs / 250;
            if (noOf250s >= 1)
            {
                points += (noOf250s * 50);
            }



            // Every four-> 1 point
            points += (battingScore.Fours);

            //Every six-> 2 point
            points += (battingScore.Sixes);

            return points;
        }


        private int CalculateBowlingPoints(BowlingScore bowlingScore)
        {
            int points = 0;
            // Every wicket-> + 25 point
            points += bowlingScore.Wickets * 25;

            switch (bowlingScore.Wickets)
            {
                case 2:
                    // 5 Bonus points for 2 wickets
                    points += 5;
                    break;
                case 3:
                    // above plus 
                    // 15 Bonus points for 3 wickets
                    // 5 + 15 = 20 points
                    points += 20;
                    break;
                case 4:
                    // above plus 
                    // 25 Bonus points for 4 wickets
                    // 20 + 25 = 45 points
                    points += 45;
                    break;
                case 5:
                    // above plus 
                    // 35 Bonus points for 5 wickets
                    // 45 + 35 = 80 points
                    points += 80;
                    break;
                case 6:
                    // above plus 
                    // 45 Bonus points for 6 wickets
                    // 80 + 45 = 125 points
                    points += 125;
                    break;
                case 7:
                    // above plus 
                    // 55 Bonus points for 7 wickets
                    // 125 + 55 = 180 points
                    points += 180;
                    break;
                case 8:
                    // above plus 
                    // 65 Bonus points for 8 wickets
                    // 180 + 65 = 245 points
                    points += 245;
                    break;
                case 9:
                    // above plus 
                    // 75 Bonus points for 9 wickets
                    // 245 + 75 = 320 points
                    points += 320;
                    break;
                case 10:
                    // above plus 
                    // 85 Bonus points for 10 wickets
                    // 320 + 85 = 180 points
                    points += 405;
                    break;

            }


            //((Balls * (Constant for match type)) -Run Given ) *2[If positive..multiplied by 2]
            int noOfBalls = 0;
            noOfBalls += (int)(bowlingScore.Overs / 1) * 6;
            noOfBalls += (int)((bowlingScore.Overs % 1.0f) * 10.0);

            int runDiff = (int)(noOfBalls * CONSTANT_RUNS_PER_BALL) - bowlingScore.Runs;
            if (runDiff >= 0)
            {
                points += (runDiff * 2);
            }
            else
            {
                points += (runDiff);
            }

            // 15 for every Maiden
            points += (bowlingScore.Maidens * 15);

            return points;
        }

        private int CalculateFieldingPoints(FieldingScore fieldingScore)
        {
            int points = 0;
            points += (fieldingScore.Catch * 15);
            points += (fieldingScore.Runout * 15);
            points += (fieldingScore.Stumped * 20);
            return points;
        }


    }
}
