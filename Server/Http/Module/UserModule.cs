using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Database.Entity;
using Nancy;
using Nancy.Security;
using Uniars.Server.Http.Response;

namespace Uniars.Server.Http.Modules
{
    public class UserModule : NancyModule
    {
        public UserModule() : base("/users")
        {
            this.RequiresAuthentication();

            Get["/"] = Index;
            Get["/{id:int}"] = Single;
            Get["/search"] = Search;
        }

        protected object Index(dynamic parameters)
        {
            List<User> models = App.Entities.Users.ToList();

            return models;
        }

        protected object Single(dynamic parameters)
        {
            User model = App.Entities.Users.Find((int)parameters.id);

            if (model == null)
            {
                return new JsonErrorResponse(404, 404, "User not found");
            }

            return model;
        }

        protected object Search(dynamic parameters)
        {
            string username = this.Request.Query["username"];
            string name = this.Request.Query["name"];

            IQueryable<User> db = App.Entities.Users;

            if (username != null)
            {
                db = App.Entities.Users.Where(User => User.Username.Contains(username));
            }

            if (name != null)
            {
                db = App.Entities.Users.Where(User => User.Name.Contains(name));
            }

            return db.ToList();
        }
    }
}
