using System;

namespace Uniars.Client.Http
{
    public class AuthException : Exception
    {
        public AuthException()
            : base("Invalid authentication credentials")
        {
        }
    }
}
