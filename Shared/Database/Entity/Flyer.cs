using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uniars.Shared.Database.Entity
{
    [Table("flyers")]
    public class Flyer
    {
        public Int32 Id { get; set; }

        [Column("public_id")]
        public String PublicId { get; set; }

        public String Name { get; set; }
    }
}
