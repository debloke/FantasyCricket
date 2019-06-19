using System;

namespace FantasyCricket.Common.Net.Exceptions
{
    public class NotAGangOwnerException : Exception
    {
        public NotAGangOwnerException(string reasonPhrase)
           : base(reasonPhrase)
        {

        }
    }
}
