﻿using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Uniars.Server.Http.Auth;
using Uniars.Server.Http.Response;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;

namespace Uniars.Server.Http.Module
{
    public class AirlineModule : BaseModule
    {
        public AirlineModule()
            : base("/airlines")
        {
            this.RequiresAuthentication();

            Get["/"] = Index;
            Get["/{id:int}"] = Single;
            Get["/search"] = Search;

            Post["/"] = CreateModel;
            Put["/{id:int}"] = UpdateModel;
            Delete["/{id:int}"] = DeleteModel;
        }

        public object Index(dynamic parameter)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<Airline> db = context.Airlines.OrderBy(Airline => Airline.Name);

                return new PaginatedResult<Airline>(db, this.perPage, this.GetCurrentPage());
            }
        }

        public object Single(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                Airline model = context.Airlines.Find((int)parameters.id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Airline not found");
                }

                return model;
            }
        }

        protected object Search(dynamic parameters)
        {
            string name = this.Request.Query["name"];
            string callsign = this.Request.Query["callsign"];
            string country = this.Request.Query["country"];

            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<Airline> db = context.Airlines;

                if (name != null)
                {
                    db = db.Where(Flyer => Flyer.Name.Contains(name));
                }

                if (callsign != null)
                {
                    db = db.Where(Flyer => Flyer.Callsign.Contains(callsign));
                }

                if (country != null)
                {
                    db = db.Where(Flyer => Flyer.Country.Contains(country));
                }

                db = db.OrderBy(Airline => Airline.Name);

                return new PaginatedResult<Airline>(db, this.perPage, this.GetCurrentPage());
            }
        }

        public object CreateModel(dynamic parameters)
        {
            this.RequiresClaims(UserIdentity.ROLE_ADMIN);

            using (Context context = new Context(App.ConnectionString))
            {
                Airline model = this.Bind<Airline>();

                context.Airlines.Add(model);
                context.SaveChanges();

                return model;
            }
        }

        protected object UpdateModel(dynamic parameters)
        {
            this.RequiresClaims(UserIdentity.ROLE_ADMIN);

            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Airline model = context.Airlines.FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Airline not found");
                }

                this.BindTo(model);

                context.SaveChanges();

                return model;
            }
        }

        protected object DeleteModel(dynamic parameters)
        {
            this.RequiresClaims(UserIdentity.ROLE_ADMIN);

            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Airline airline = context.Airlines.FirstOrDefault(m => m.Id == id);

                if (airline == null)
                {
                    return new JsonErrorResponse(404, 404, "Airline not found");
                }

                context.Airlines.Remove(airline);
                context.SaveChanges();
            }

            return HttpStatusCode.OK;
        }
    }
}