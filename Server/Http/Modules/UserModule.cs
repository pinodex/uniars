using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Database.Entity;
using Newtonsoft.Json;
using Nancy;

namespace Uniars.Server.Http.Modules
{
    public class UserModule : NancyModule
    {
        public UserModule() : base("/users")
        {
            Get["/"] = Index;
            Get["/{id:int}"] = Single;
            Get["/search"] = Search;
        }

        protected object Index(dynamic parameters)
        {
            List<User> users = App.Entities.User.ToList();

            return users;
        }

        protected object Single(dynamic parameters)
        {
            User user = App.Entities.User.Find((int)parameters.id);

            if (user == null)
            {
                return new ErrorJsonResponse(404, 404, "User not found");
            }

            return user;
        }

        protected object Search(dynamic parameters)
        {
            string type = this.Request.Query["type"];
            string query = this.Request.Query["query"];

            List<User> users = new List<User>();

            if (type == "username")
            {
                users = App.Entities.User.Where(User => User.Username.Contains(query)).ToList();
            }

            if (type == "name")
            {
                users = App.Entities.User.Where(User => User.Name.Contains(query)).ToList();
            }

            return users;
        }
    }
}
