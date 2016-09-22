using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uniars.Shared.Database.Entity
{
    [Table("passengers")]
    public class Passenger
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Prefix { get; set; }

        [Column("given_name")]
        public string GivenName { get; set; }

        [Column("family_name")]
        public string FamilyName { get; set; }

        [Column("middle_name")]
        public string MiddleName { get; set; }

        public string Suffix { get; set; }

        [Column("display_name")]
        public string DisplayName { get; set; }

        public int Gender { get; set; }

        public PassengerAddress Address { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
