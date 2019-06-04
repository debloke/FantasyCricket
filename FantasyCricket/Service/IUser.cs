using FantasyCricket.Models;
using System;

namespace FantasyCricket.Service
{
    public interface IUser
    {
        void CreateUser(string username, string password, string displayName);
        MagicKey LoginUser(string username, string password);

        void SaveTeam(UserTeam userTeam, Guid magicKey);

        UserTeam GetTeam(Guid magicKey);

        UserTeam GetLastTeam(string username);

        UserTeamHistory[] GetHistoryTeam(string username);

        string[] GetUsers();
    }
}
