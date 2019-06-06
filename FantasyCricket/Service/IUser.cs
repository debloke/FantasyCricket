using FantasyCricket.Models;
using System;

namespace FantasyCricket.Service
{
    public interface IUser
    {
        void CreateUser(string username, string password, string displayName);
        MagicKey LoginUser(string username, string password);

        MagicKey LoginUser(Guid magicKey);

        MagicKey LoginUser(string username);

        void LogoutUser(string username);

        void SaveTeam(UserTeam userTeam, string username);

        UserTeam GetTeam(string username);

        UserTeam GetLastTeam(string username);

        UserTeamHistory[] GetHistoryTeam(string username);

        string[] GetUsers();
    }
}
