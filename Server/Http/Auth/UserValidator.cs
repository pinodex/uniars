using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Security;
using Nancy.Authentication.Basic;
using Uniars.Shared.Database.Entity;
using Uniars.Shared.Foundation;
using Uniars.Shared.Database;

namespace Uniars.Server.Http.Auth
{
    class UserValidator : IUserValidator
    {
        public IUserIdentity Validate(string username, string password)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                User user = context.Users.FirstOrDefault(User => User.Username == username);

                if (user == null)
                {
                    return null;
                }

                if (!Hash.Check(password, user.Password))
                {
                    return null;
                }

                return new UserIdentity
                {
                    User = user,
                    UserName = username,
                    Claims = user.Role.Split(',')
                };
            }
        }
    }
}
