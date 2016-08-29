using System;
using System.Linq;
using Uniars.Data;

namespace Uniars.Core.Auth
{
    public class Auth
    {
        /// <summary>
        /// WAttempt login with username and password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>User</returns>
        public static Data.Entity.User Login(string username, string password)
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
