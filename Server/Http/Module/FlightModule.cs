using System.Data.Entity;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Uniars.Server.Http.Auth;
using Uniars.Server.Http.Response;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;

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

        protected object Search(dynamic parameters)
        {
            string code = this.Request.Query["code"];

            int airlineId = 0;
            int sourceId = 0;
            int destinationId = 0;

            try
            {
                airlineId = int.Parse(this.Request.Query["airline"]);
                sourceId = int.Parse(this.Request.Query["source"]);
                destinationId = int.Parse(this.Request.Query["destination"]);
            }
            catch { }

            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<Flight> db = context.Flights;

                if (code != null)
                {
                    db = db.Where(m => m.Code == code);
                }

                if (airlineId != 0)
                {
                    db = db.Where(m => m.AirlineId == airlineId);
                }

                if (sourceId != 0)
                {
                    db = db.Where(m => m.SourceId == sourceId);
                }

                if (destinationId != 0)
                {
                    db = db.Where(m => m.DestinationId == destinationId);
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

                return model;
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

                return model;
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