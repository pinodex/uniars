using System;
using System.Linq;
using System.Configuration;
using System.Data.Entity;
using MySql.Data.Entity;
using Uniars.Shared.Database.Entity;

namespace Uniars.Shared.Database
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Passenger> Passengers { get; set; }

        public DbSet<PassengerContact> PassengerContacts { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Airline> Airlines { get; set; }

        public DbSet<Airport> Airports { get; set; }

        public DbSet<Route> Routes { get; set; }

        public DbSet<Flight> Flights { get; set; }

        public DbSet<Book> Books { get; set; }

        public Context(string connectionString) : base(connectionString)
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Passenger>()
                .HasMany(p => p.Contacts);

            modelBuilder.Entity<Book>()
                .HasMany(m => m.Passengers)
                .WithMany()
                .Map(x =>
                {
                    x.MapLeftKey("book_id");
                    x.MapRightKey("passenger_id");
                    x.ToTable("book_passengers");
                });
        }

        public override int SaveChanges()
        {
            DateTime now = DateTime.UtcNow;

            this.ChangeTracker.Entries<ITrackedEntity>().ToList().ForEach(e =>
            {
                if (e.State == EntityState.Added)
                {
                    e.Entity.CreatedAt = now;
                }

                if (e.State == EntityState.Modified)
                {
                    ITrackedEntity dbValues = (ITrackedEntity)e.GetDatabaseValues().ToObject();

                    if (dbValues != null)
                    {
                        e.Entity.CreatedAt = dbValues.CreatedAt;
                    }

                    e.Entity.UpdatedAt = now;
                }
            });

            return base.SaveChanges();
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
