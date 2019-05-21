using FantasyCricket.Models;

namespace FantasyCricket.Service
{
    public interface ISeriesInfo
    {
        Match[] GetMatches();

        Match[] GetUnAssignedMatches();

        void CreateSeries(string seriesName);

        void AddMatch(Match match);

    }
}
