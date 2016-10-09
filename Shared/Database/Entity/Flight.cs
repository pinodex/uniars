using System;
using System.ComponentModel.DataAnnotations.Schema;
using Uniars.Shared.Foundation;
namespace Uniars.Shared.Database.Entity
{
    [Table("flights")]
    public class Flight
    {
        public int Id { get; set; }

        public string Code { get; set; }

        [Column("airline_id")]
        public int AirlineId { get; set; }

        [Column("source_id")]
        public int SourceId { get; set; }

        [Column("destination_id")]
        public int DestinationId { get; set; }

        [ForeignKey("AirlineId")]
        public Airline Airline { get; set; }

        [ForeignKey("SourceId")]
        public Airport Source { get; set; }

        [ForeignKey("DestinationId")]
        public Airport Destination { get; set; }

        [Column("departure_date")]
        public DateTime DepartureDate { get; set; }

        [Column("return_date")]
        public DateTime? ReturnDate { get; set; }

        public int Type { get; set; }

        [Column("has_departed")]
        public bool HasDeparted { get; set; }

        public string GenerateCode()
        {
            DateTime now = DateTime.Now;

            string year = now.Year.ToString();
            string month = now.Month.ToString().PadLeft(2, '0');

            this.Code = string.Format("{0}-{1}", year, Helper.GetSecureRandomAlphaNumeric(6).ToUpper());

            return this.Code;
        }
    }
}
