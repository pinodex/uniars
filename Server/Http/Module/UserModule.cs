using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Database.Entity;
using Nancy;
using Nancy.Security;
using Uniars.Server.Http.Response;
using Uniars.Shared.Database;

namespace Uniars.Server.Http.Module
{
    public class UserModule : BaseModule
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
            IQueryable<User> db = App.Entities.Users.OrderBy(User => User.Id);

            return new PaginatedResult<User>(db, this.perPage, this.GetCurrentPage());
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

            db = db.OrderBy(User => User.Id);

            return new PaginatedResult<User>(db, this.perPage, this.GetCurrentPage());
        }
    }
}
