using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uniars.Shared.Database.Entity
{
    [Table("countries")]
    public class Country
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
