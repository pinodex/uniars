using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
