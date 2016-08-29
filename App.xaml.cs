using System.Data;
using System.Windows;
using Uniars.Data;
using Uniars.Core;

namespace Uniars
{
    public partial class App : Application
    {
        public static Data.Entity.User CurrentUser;

        public static Config Config;

        public static Entities Entities;

        public App()
        {
            App.Config = new Config("Config.xml");
            App.Entities = new Entities();
        }
    }
}
