using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Nancy.Security;
using Uniars.Shared.Database;
using Uniars.Server.Http.Auth;
using Nancy;
using Uniars.Shared.Model;


namespace Uniars.Server.Http.Module
{
    public class MainModule : BaseModule
    {
        public MainModule() : base("/")
        {
            this.RequiresAuthentication();

            Get["/"] = Index;
            Get["/auth"] = Auth;
            Get["/stats"] = Stats;
            Get["/health"] = Health;
        }

        public object Index(dynamic parameters)
        {
            return Response.AsText(string.Format("UNIARS-Server/{0}", App.Version), "text/plain");
        }

        public object Auth(dynamic parameters)
        {
            return ((UserIdentity)this.Context.CurrentUser).User;
        }

        public object Stats(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                return new StatsModel
                {
                    AirlineCount = context.Airlines.Count(),
                    AirportCount = context.Airports.Count(),
                    FlightCount = context.Flights.Count(),
                    PassengerCount = context.Passengers.Count(),

                    LatestFlight = context.Flights
                        .Include(m => m.Airline)
                        .Include(m => m.Source)
                        .Include(m => m.Destination)
                        .OrderByDescending(m => m.Id)
                        .FirstOrDefault(),

                    LatestPassenger = context.Passengers
                        .OrderByDescending(m => m.Id)
                        .FirstOrDefault()
                };
            }
        }

        public object Health(dynamic parameters)
        {
            return HttpStatusCode.OK;
        }
    }
}
