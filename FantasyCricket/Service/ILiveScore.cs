using FantasyCricket.Models;
using System.Collections.Concurrent;

namespace FantasyCricket.Service
{
    public interface ILiveScore
    {
        ConcurrentDictionary<int, Points[]> GetScores();

        void LiveScoreCheckTimerEvent(object state);


        UserPoints[] GetUserPoints();

        UserPoints[] GetUserPoints(string gang);

    }
}
