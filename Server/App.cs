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
using SWF = System.Windows.Forms;
using System.Reflection;

namespace Uniars.Server
{
    public class App : Application
    {
        public static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public const string CONFIG_FILE = "Config.json";

        public static BaseModel Config;

        public static Http.Server Server;

        public static Entities Entities;

        [STAThread]
        public static int Main()
        {
            Config = (BaseModel) JsonConfig.Load<BaseModel>(CONFIG_FILE);

            Server = new Http.Server(App.Config.Server.Host, App.Config.Server.Port);
            Entities = new Entities();

            Application app = new Application();
            app.Run(new MainWindow());

            return 0;
        }
    }
}
