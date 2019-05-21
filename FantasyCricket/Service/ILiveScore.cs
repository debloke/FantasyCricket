using FantasyCricket.Models;

namespace FantasyCricket.Service
{
    public interface ILiveScore
    {
        Points[] GetScore(int uniqueId);

    }
}
