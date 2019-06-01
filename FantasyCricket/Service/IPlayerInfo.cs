using FantasyCricket.Models;

namespace FantasyCricket.Service
{
    public interface IPlayerInfo
    {
        Player[] GetPlayers(TeamType teamType);

        void UpdateIplPlayers(int uniqueId);
        void UpdateCountryPlayers(int uniqueId);

        void DeletePlayer(int uniqueId, TeamType teamType);

        void UpdatePlayerCost(int uniqueId,int cost, Role role, TeamType teamType);
    }
}
