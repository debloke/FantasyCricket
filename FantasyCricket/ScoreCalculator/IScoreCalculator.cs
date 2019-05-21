using FantasyCricket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyCricket.ScoreCalculator
{
    public interface IScoreCalculator
    {
        Dictionary<int, Points> CalculateScore(PlayerScoresResponse playerScoresResponse);
    }
}
