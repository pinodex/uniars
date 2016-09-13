using System;
using System.Linq;
using Uniars.Client.Data;

namespace Uniars.Client.Core.Auth
{
    public class Auth
    {
        /// <summary>
        /// WAttempt login with username and password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Logged in user</returns>
        public static Data.Entity.User Login(String username, String password)
        {
            IQueryable<Data.Entity.User> result = App.Entities.User.Take(1).Where(User => User.Username == username);

            if (result.Count() == 0)
            {
                return null;
            }

            Data.Entity.User user = result.ToArray().First();

            if (Hash.Check(password, user.Password))
            {
                user.LastLogin = DateTime.Now;
                App.Entities.SaveChanges();

                return user;
            }

            return null;
        }
    }
}
