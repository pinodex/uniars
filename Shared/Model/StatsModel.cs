using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Database.Entity;

namespace Uniars.Shared.Model
{
    public class StatsModel
    {
        public int AirlineCount { get; set; }

        public int AirportCount { get; set; }

        public int FlightCount { get; set; }

        public int PassengerCount { get; set; }

        public Flight LatestFlight { get; set; }

        public Passenger LatestPassenger { get; set; }
    }
}
