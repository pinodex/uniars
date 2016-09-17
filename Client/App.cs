using System;
using System.Data;
using System.Linq;
using System.Windows;
using Uniars.Client.UI;
using System.Threading;

namespace Uniars.Client
{
    public partial class App : Application
    {
        [STAThread]
        public static int Main()
        {
            Application app = new Application();
            app.Run(new LoginWindow());

            return 0;
        }
    }
}
