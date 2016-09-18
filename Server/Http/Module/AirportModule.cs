using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;
using Uniars.Shared.Foundation;
using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using Uniars.Server.Http.Response;

namespace Uniars.Server.Http.Modules
{
    public class AirportModule : NancyModule
    {
        protected int perPage = 100;

        public AirportModule()
            : base("/airports")
        {
            this.RequiresAuthentication();

            Get["/"] = Index;
            Get["/{id:int}"] = Single;
            Get["/search"] = Search;
        }

        public object Index(dynamic parameter)
        {
            int page = 1;

            try
            {
                page = int.Parse(this.Request.Query["page"]);
            }
            catch { }

            IQueryable<Airport> db = App.Entities.Airports.OrderBy(Airline => Airline.Id);

            return new PaginatedResult<Airport>(db, perPage, page);
        }

        public object Single(dynamic parameters)
        {
            Airport model = App.Entities.Airports.Find((int)parameters.id);

            if (model == null)
            {
                return new JsonErrorResponse(404, 404, "Airline not found");
            }

            return model;
        }

        protected object Search(dynamic parameters)
        {
            string name = this.Request.Query["name"];
            string country = this.Request.Query["country"];
            string city = this.Request.Query["city"];
            string timezone = this.Request.Query["timezone"];

            IQueryable<Airport> db = App.Entities.Airports;

            if (name != null)
            {
                db = db.Where(Flyer => Flyer.Name.Contains(name));
            }

            if (country != null)
            {
                db = db.Where(Flyer => Flyer.Country.Contains(country));
            }

            if (city != null)
            {
                db = db.Where(Flyer => Flyer.City.Contains(city));
            }

            if (timezone != null)
            {
                db = db.Where(Flyer => Flyer.Timezone.Contains(timezone));
            }

            int page = 1;

            try
            {
                page = int.Parse(this.Request.Query["page"]);
            }
            catch { }

            db = db.OrderBy(Airline => Airline.Id);

            return new PaginatedResult<Airport>(db, perPage, page);
        }
    }
}