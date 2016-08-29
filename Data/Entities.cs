using System;
using System.Configuration;
using System.Data.Entity;
using MySql.Data.Entity;

namespace Uniars.Data
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Entities : DbContext
    {
        public Entities() : base(GetConnectionString()) {}

        protected static String GetConnectionString()
        {
            String host, username, password, name;

            host = App.Config.Get("Database.Host");
            username = App.Config.Get("Database.Username");
            password = App.Config.Get("Database.Password");
            name = App.Config.Get("Database.Name");
            
            return String.Format("server={0};uid={1};password={2};database={3}", host, username, password, name);
        }

        public DbSet<Entity.User> User { get; set; }
    }
}
