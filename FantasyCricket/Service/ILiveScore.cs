using FantasyCricket.Models;
using System.Collections.Concurrent;

namespace FantasyCricket.Service
{
    public interface ILiveScore
    {
        ConcurrentDictionary<int, Points[]> GetScores();

    }
}
