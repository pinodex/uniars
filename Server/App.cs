using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Net;
using System.IO;
using System.Threading;
using Nancy.Hosting.Self;
using Uniars.Server.Database;
using Uniars.Server.UI;
using Uniars.Server.Core.Config;
using Uniars.Shared.Foundation.Config;

namespace Uniars.Server
{
    public class App : Application
    {
        public const string VERSION = "1.0.0";

        public static BaseModel Config;

        public static Entities Entities;

        [STAThread]
        public static int Main()
        {
            /*
            HostConfiguration config = new HostConfiguration();
            config.UrlReservations.CreateAutomatically = true;

            NancyHost host = new NancyHost(config, new Uri("http://localhost:8000"));

            host.Start();

            Entities = new Entities();
            */

            Config = (BaseModel) JsonConfig.Load<BaseModel>("Config.json");

            Application app = new Application();
            app.Run(new MainWindow());

            return 0;
        }
    }
}
