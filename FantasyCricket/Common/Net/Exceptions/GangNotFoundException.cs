using System;

namespace FantasyCricket.Common.Net.Exceptions
{
    public class GangNotFoundException : Exception
    {
        public GangNotFoundException(string reasonPhrase)
           : base(reasonPhrase)
        {

        }
    }
}
