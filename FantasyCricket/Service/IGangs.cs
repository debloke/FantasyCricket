using FantasyCricket.Models;

namespace FantasyCricket.Service
{
    public interface IGangs
    {
        Gang[] GetGangs(string username, int seriesid);

        void CreateGang(Gang gang);

        void RemoveGang(int gangid, string owner);
        void AddToGang(int gangid, string[] usernames, string owner);

        void AcceptGangMembership(int gangid,string username);

        void RemoveFromGang(int gangid, string[] usernames, string owner);
    }
}
