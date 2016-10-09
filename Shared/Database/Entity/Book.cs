using System;
using System.ComponentModel.DataAnnotations.Schema;
using Uniars.Shared.Foundation;
using System.Collections.Generic;
namespace Uniars.Shared.Database.Entity
{
    [Table("books")]
    public class Book
    {
        public int Id { get; set; }

        [Column("flight_id")]
        public int FlightId { get; set; }

        [ForeignKey("FlightId")]
        public Flight Flight { get; set; }

        public List<Passenger> Passengers { get; set; }
    }
}
