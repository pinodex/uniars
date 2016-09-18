using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uniars.Shared.Database.Entity
{
    [Table("airlines")]
    public class Airline
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string IATA { get; set; }

        public string ICAO { get; set; }

        public string Callsign { get; set; }

        public string Country { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }
    }
}
