using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uniars.Shared.Database.Entity
{
    [Table("users")]
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }
    }
}
