using System;
using System.Configuration;
using System.Data.Entity;
using MySql.Data.Entity;

namespace Uniars.Shared.Database
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Entities : DbContext
    {
        public Entities(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Try to open connection to database
        /// </summary>
        /// <returns>Connection status</returns>
        public bool OpenConnection()
        {
            try
            {
                this.Database.Connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public DbSet<Entity.User> Users { get; set; }

        public DbSet<Entity.Flyer> Flyers { get; set; }

        public DbSet<Entity.Airline> Airlines { get; set; }

        public DbSet<Entity.Airport> Airports { get; set; }

        public DbSet<Entity.Route> Routes { get; set; }
    }
}
