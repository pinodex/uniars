using System.Data.Entity;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Uniars.Server.Http.Auth;
using Uniars.Server.Http.Response;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;
using System;

namespace Uniars.Server.Http.Module
{
    public class FlightModule : BaseModule
    {
        public FlightModule()
            : base("/flights")
        {
            this.RequiresAuthentication();

            Get["/"] = Index;
            Get["/{id:int}"] = Single;
            Get["/{id}"] = SingleCode;
            Get["/search"] = Search;

            Post["/"] = CreateModel;
            Put["/{id:int}"] = UpdateModel;
            Delete["/{id:int}"] = DeleteModel;
        }

        public object Index(dynamic parameter)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<Flight> db = context.Flights
                    .Include(m => m.Airline)
                    .Include(m => m.Source)
                    .Include(m => m.Destination)
                    .OrderBy(m => m.Id);

                return new PaginatedResult<Flight>(db, this.perPage, this.GetCurrentPage());
            }
        }

        public object Single(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                int id = (int)parameters.id;

                Flight model = context.Flights
                    .Include(m => m.Airline)
                    .Include(m => m.Source)
                    .Include(m => m.Destination)
                    .FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Flight not found");
                }

                return model;
            }
        }

        public object SingleCode(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                string code = parameters.code;

                Flight model = context.Flights
                    .Include(m => m.Airline)
                    .Include(m => m.Source)
                    .Include(m => m.Destination)
                    .FirstOrDefault(m => m.Code == code);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Flight not found");
                }

                return model;
            }
        }

        protected object Search(dynamic parameters)
        {
            string airline = this.Request.Query["airline"];
            string source = this.Request.Query["source"];
            string destination = this.Request.Query["destination"];

            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<Flight> db = context.Flights
                    .Include(m => m.Airline)
                    .Include(m => m.Source)
                    .Include(m => m.Destination);

                if (airline != null)
                {
                    db = db.Where(m => m.Airline.Name.Contains(airline));
                }

                if (source != null)
                {
                    db = db.Where(m => m.Source.Name.Contains(source));
                }

                if (destination != null)
                {
                    db = db.Where(m => m.Destination.Name.Contains(destination));
                }

                db = db.OrderBy(Airline => Airline.Id);

                return new PaginatedResult<Flight>(db, this.perPage, this.GetCurrentPage());
            }
        }

        public object CreateModel(dynamic parameters)
        {
            this.RequiresClaims(UserIdentity.ROLE_ADMIN);

            using (Context context = new Context(App.ConnectionString))
            {
                Flight model = this.Bind<Flight>(
                    m => m.Id,
                    m => m.Code,
                    m => m.Airline,
                    m => m.Source,
                    m => m.Destination
                );

                model.GenerateCode();

                context.Flights.Add(model);
                context.SaveChanges();

                return context.Flights
                    .Include(m => m.Airline)
                    .Include(m => m.Source)
                    .Include(m => m.Destination)
                    .FirstOrDefault(m => m.Id == model.Id);
            }
        }

        protected object UpdateModel(dynamic parameters)
        {
            this.RequiresClaims(UserIdentity.ROLE_ADMIN);

            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Flight model = context.Flights.FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Flight not found");
                }

                this.BindTo(model,
                    m => m.Id,
                    m => m.Code,
                    m => m.Airline,
                    m => m.Source,
                    m => m.Destination
                );

                context.SaveChanges();

                return context.Flights
                    .Include(m => m.Airline)
                    .Include(m => m.Source)
                    .Include(m => m.Destination)
                    .FirstOrDefault(m => m.Id == model.Id);
            }
        }

        protected object DeleteModel(dynamic parameters)
        {
            this.RequiresClaims(UserIdentity.ROLE_ADMIN);

            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Flight model = context.Flights.FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Flight not found");
                }

                context.Flights.Remove(model);
                context.SaveChanges();
            }

            return HttpStatusCode.OK;
        }
    }
}