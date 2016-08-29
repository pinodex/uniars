using System;
using System.Data;
using System.Windows;
using Uniars.Core;
using Uniars.Data;
using Uniars.UI;
using System.Threading;

namespace Uniars
{
    public partial class App : Application
    {
        public static Data.Entity.User CurrentUser;

        public static Config Config;

        public static Entities Entities;

        [STAThread]
        public static int Main()
        {
            Config = new Config("Config.xml");
            Entities = new Entities();

            while (!Entities.IsConnected())
            {
                MessageBoxResult result = MessageBox.Show(
                    "Unable to connect to database. Retry connection?",
                    "Error",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Error
                );

                if (result.Equals(MessageBoxResult.No))
                {
                    return 1;
                }
            }

            Application app = new Application();
            app.Run(new LoginWindow());

            return 0;
        }
    }
}
