using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNIARS.Auth
{
    public class User
    {
        public string Name { get; protected set; }

        public User()
        {
            this.Name = "Unknown User";
        }

        public User(string Name)
        {
            this.Name = Name;
        }
    }
}
