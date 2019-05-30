using FantasyCricket.Models;
using System.Collections.Generic;

namespace FantasyCricket.ScoreCalculator
{
    public class PointsCalculator
    {
        public static int CalculatePoints(Points[] points, UserTeam userTeam)
        {
            int totalPoints = 0;
            List<int> playedIds = new List<int>(userTeam.PlayerIds);

            foreach (Points point in points)
            {
                if (playedIds.Contains(point.PlayerId))
                {
                    totalPoints += (point.BattingPoints + point.BowlingPoints + point.FieldingPoints + point.Bonus);
                }
                if (point.PlayerId == userTeam.BattingCaptain)
                {
                    totalPoints += point.BattingPoints;
                }
                if (point.PlayerId == userTeam.BowlingCaptain)
                {
                    totalPoints += point.BowlingPoints;
                }
                if (point.PlayerId == userTeam.FieldingCaptain)
                {
                    totalPoints += point.FieldingPoints;
                }
            }



            return totalPoints;
        }
    }
}
