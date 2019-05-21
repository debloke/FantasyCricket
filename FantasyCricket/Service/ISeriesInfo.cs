using FantasyCricket.Models;

namespace FantasyCricket.Service
{
    public interface ISeriesInfo
    {
        Match[] GetMatches();

        Series[] GetSeries();

        Match[] GetUnAssignedMatches();

        void CreateSeries(string seriesName);

        void AddMatch(Match match);

    }
}
