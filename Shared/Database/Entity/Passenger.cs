using System;
using System.ComponentModel.DataAnnotations.Schema;
using Uniars.Shared.Foundation;
using System.Collections.Generic;

namespace Uniars.Shared.Database.Entity
{
    [Table("passengers")]
    public class Passenger : ITrackedEntity
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

        public List<PassengerContact> Contacts { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public string GenerateCode()
        {
            DateTime now = DateTime.Now;

            string year = now.Year.ToString();
            string month = now.Month.ToString().PadLeft(2, '0');

            this.Code = string.Format("{0}-{1}-{2}", year, month, Helper.GetSecureRandomAlphaNumeric(6).ToUpper());

            return this.Code;
        }
    }
}
