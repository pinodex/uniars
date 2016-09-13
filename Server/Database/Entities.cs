using System;
using System.Configuration;
using System.Data.Entity;
using MySql.Data.Entity;

namespace Uniars.Server.Database
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Entities : DbContext
    {
        public Entities() : base(GetConnectionString()) {}

        /// <summary>
        /// Get database connection string
        /// </summary>
        protected static String GetConnectionString()
        {
            String host, username, password, name;

            host = "";
            username = "";
            password = "";
            name = "";
            
            return String.Format("server={0};uid={1};password={2};database={3}", host, username, password, name);
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

        public DbSet<Entity.User> User { get; set; }
    }
}
