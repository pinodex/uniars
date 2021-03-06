﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Security;
using Uniars.Shared.Database.Entity;

namespace Uniars.Server.Http.Auth
{
    public class UserIdentity : IUserIdentity
    {
        public const string ROLE_ADMIN = "admin";

        public const string ROLE_USER = "user";

        public User User { get; set; }

        public string UserName { get; set; }

        public IEnumerable<string> Claims { get; set; }
    }
}
