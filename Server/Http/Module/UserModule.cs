﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Database.Entity;
using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using Uniars.Server.Http.Response;
using Uniars.Shared.Database;
using Uniars.Shared.Foundation;
using Uniars.Server.Http.Auth;

namespace Uniars.Server.Http.Module
{
    public class UserModule : BaseModule
    {
        public UserModule() : base("/users")
        {
            this.RequiresAuthentication();
            this.RequiresClaims(UserIdentity.ROLE_ADMIN);

            Get["/"] = Index;
            Get["/{id:int}"] = Single;
            Get["/search"] = Search;

            Post["/"] = CreateModel;
            Put["/{id:int}"] = UpdateModel;
            Delete["/{id:int}"] = DeleteModel;
        }

        protected object Index(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<User> db = context.Users.OrderBy(User => User.Id);

                return new PaginatedResult<User>(db, this.perPage, this.GetCurrentPage());
            }
        }

        protected object Single(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                User model = context.Users.Find((int)parameters.id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "User not found");
                }

                model.Password = null;

                return model;
            }
        }

        protected object Search(dynamic parameters)
        {
            string username = this.Request.Query["username"];
            string name = this.Request.Query["name"];

            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<User> db = context.Users;

                if (username != null)
                {
                    db = context.Users.Where(User => User.Username.Contains(username));
                }

                if (name != null)
                {
                    db = context.Users.Where(User => User.Name.Contains(name));
                }

                db = db.OrderBy(User => User.Id);

                return new PaginatedResult<User>(db, this.perPage, this.GetCurrentPage());
            }
        }

        public object CreateModel(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                User user = this.Bind<User>();

                user.Password = Hash.Make(user.Password);

                context.Users.Add(user);
                context.SaveChanges();

                return user;
            }
        }

        protected object UpdateModel(dynamic parameters)
        {
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                User user = context.Users.FirstOrDefault(m => m.Id == id);

                if (user == null)
                {
                    return new JsonErrorResponse(404, 404, "User not found");
                }

                string currentPassword = user.Password;

                this.BindTo(user);

                if (user.Password == null)
                {
                    user.Password = currentPassword;
                }
                else
                {
                    user.Password = Hash.Make(user.Password);
                }

                context.SaveChanges();

                return user;
            }
        }

        protected object DeleteModel(dynamic parameters)
        {
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                User user = context.Users.FirstOrDefault(m => m.Id == id);

                if (user == null)
                {
                    return new JsonErrorResponse(404, 404, "User not found");
                }

                context.Users.Remove(user);
                context.SaveChanges();
            }

            return HttpStatusCode.OK;
        }
    }
}
