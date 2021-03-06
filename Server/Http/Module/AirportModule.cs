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
    public class AirportModule : BaseModule
    {
        public AirportModule()
            : base("/airports")
        {
            this.RequiresAuthentication();

            Get["/"] = Index;
            Get["/{id:int}"] = Single;

            Post["/"] = CreateModel;
            Put["/{id:int}"] = UpdateModel;
            Delete["/{id:int}"] = DeleteModel;
        }

        public object Index(dynamic parameter)
        {
            string name = this.Request.Query["name"];
            string country = this.Request.Query["country"];
            string city = this.Request.Query["city"];
            string timezone = this.Request.Query["timezone"];

            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<Airport> db = context.Airports;

                if (name != null && name != string.Empty)
                {
                    db = db.Where(Model => Model.Name.Contains(name));
                }

                if (country != null && country != string.Empty)
                {
                    db = db.Where(Model => Model.Country.Contains(country));
                }

                if (city != null && city != string.Empty)
                {
                    db = db.Where(Model => Model.City.Contains(city));
                }

                db = db.OrderBy(Airline => Airline.Name);

                return new PaginatedResult<Airport>(db, this.perPage, this.GetCurrentPage());
            }
        }

        public object Single(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                Airport model = context.Airports.Find((int)parameters.id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Airline not found");
                }

                return model;
            }
        }

        public object CreateModel(dynamic parameters)
        {
            this.RequiresClaims(UserIdentity.ROLE_ADMIN);

            using (Context context = new Context(App.ConnectionString))
            {
                Airport airport = this.Bind<Airport>();

                context.Airports.Add(airport);
                context.SaveChanges();

                return airport;
            }
        }

        protected object UpdateModel(dynamic parameters)
        {
            this.RequiresClaims(UserIdentity.ROLE_ADMIN);

            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Airport airport = context.Airports.FirstOrDefault(m => m.Id == id);

                if (airport == null)
                {
                    return new JsonErrorResponse(404, 404, "Airport not found");
                }

                this.BindTo(airport);

                context.SaveChanges();

                return airport;
            }
        }

        protected object DeleteModel(dynamic parameters)
        {
            this.RequiresClaims(UserIdentity.ROLE_ADMIN);

            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Airport model = context.Airports.FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Airline not found");
                }

                context.Airports.Remove(model);
                context.SaveChanges();
            }

            return HttpStatusCode.OK;
        }
    }
}