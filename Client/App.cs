using System;
using System.Data;
using System.Linq;
using System.Windows;
using Uniars.Client.Core;
using Uniars.Client.Data;
using Uniars.Client.Data.Entity;
using Uniars.Client.UI;
using System.Threading;

namespace Uniars.Client
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

            while (!Entities.OpenConnection())
            {
                MessageBoxResult result = MessageBox.Show(
                    "Database connection failed. Retry?",
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
            Window startupWindow;

            switch (Config.Get("Application.MockMode"))
            {
                case "true":
                    SetUpMockUser();
                    startupWindow = new MainWindow();

                    break;

                default:
                    startupWindow = new LoginWindow();
                    break;
            }

            app.Run(startupWindow);
            return 0;
        }

        private static void SetUpMockUser()
        {
            IQueryable<User> testUserQuery = Entities.User.Take(1).Where(User => User.Username == "test");

            if (testUserQuery.Count() == 0)
            {
                User testUser = new User()
                {
                    Username = "test",
                    Name = "Test User",
                    Password = Hash.Make("test"),
                    Role = "admin",
                };

                Entities.User.Add(testUser);
                Entities.SaveChanges();

                CurrentUser = testUser;
                return;
            }

            CurrentUser = testUserQuery.ToArray().First();
        }
    }
}
