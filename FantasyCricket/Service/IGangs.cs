using FantasyCricket.Models;
using System;

namespace FantasyCricket.Service
{
    public interface IGangs
    {
        Gang[] GetGangs(Guid magicKey);
    }
}
