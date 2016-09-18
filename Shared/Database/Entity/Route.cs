using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uniars.Shared.Database.Entity
{
    [Table("routes")]
    public class Route
    {
        public int Id { get; set; }

        public string Source { get; set; }

        [Column("source_id")]
        public int SourceId { get; set; }

        public string Destination { get; set; }

        [Column("destination_id")]
        public int DestinationId { get; set; }

        public string Airline { get; set; }

        [Column("is_codeshare")]
        public bool IsCodeshare { get; set; }

        public int Stops { get; set; }

        public string Equipment { get; set; }
    }
}