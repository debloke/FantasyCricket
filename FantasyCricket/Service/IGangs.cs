using FantasyCricket.Models;
using System;

namespace FantasyCricket.Service
{
    public interface IGangs
    {
        Gang[] GetGangs(string username);

        void CreateGang(Gang gang);
    }
}
