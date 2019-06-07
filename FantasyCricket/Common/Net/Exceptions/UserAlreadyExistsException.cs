using System;

namespace FantasyCricket.Common.Net.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string reasonPhrase)
           : base(reasonPhrase)
        {

        }
    }
}
