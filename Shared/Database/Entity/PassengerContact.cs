using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uniars.Shared.Database.Entity
{
    [Table("passenger_contacts")]
    public class PassengerContact : ITrackedEntity
    {
        public int Id { get; set; }

        [Column("passenger_id")]
        public int PassengerId { get; set; }

        [Column("line_1")]
        public string Line1 { get; set; }

        [Column("line_2")]
        public string Line2 { get; set; }

        [Column("line_3")]
        public string Line3 { get; set; }

        [Column("city")]
        public string City { get; set; }

        [Column("state")]
        public string State { get; set; }

        [Column("province")]
        public string Province { get; set; }

        [Column("country_id")]
        public string CountryId { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }

        [Column("postal_code")]
        public string PostalCode { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
