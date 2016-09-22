using System;
using System.Configuration;
using System.Data.Entity;
using MySql.Data.Entity;

namespace Uniars.Shared.Database
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Entities : DbContext
    {
        public DbSet<Entity.User> Users { get; set; }

        public DbSet<Entity.Passenger> Passengers { get; set; }

        public DbSet<Entity.PassengerAddress> PassengerAddresses { get; set; }

        public DbSet<Entity.Country> Countries { get; set; }

        public DbSet<Entity.Airline> Airlines { get; set; }

        public DbSet<Entity.Airport> Airports { get; set; }

        public DbSet<Entity.Route> Routes { get; set; }

        public Entities(string connectionString) : base(connectionString)
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entity.Passenger>()
                .HasRequired(p => p.Address);

            modelBuilder.Entity<Entity.PassengerAddress>()
                .HasRequired(p => p.Country);
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
    }
}
