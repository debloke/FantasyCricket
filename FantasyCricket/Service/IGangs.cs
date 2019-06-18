using FantasyCricket.Models;
using System;

namespace FantasyCricket.Service
{
    public interface IGangs
    {
        Gang[] GetGangs(string username, int seriesid);

        void CreateGang(Gang gang);

        void AddToGang(int gangid,string[] usernames);
    }
}
