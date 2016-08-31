using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uniars.Data.Entity
{
    [Table("users")]
    public class User
    {
        public Int32 Id { get; set; }

        public String Name { get; set; }

        public String Username { get; set; }

        public String Password { get; set; }

        public String Role { get; set; }

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }
    }
}
