using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniars.Shared.Model
{
    public class BookModel
    {
        public int FlightId { get; set; }

        public List<int> PassengerIds { get; set; }
    }
}
