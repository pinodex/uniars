using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Uniars.Client.UI;
using Uniars.Client.Http;
using Uniars.Client.Core.Config;
using Uniars.Shared.Foundation.Config;

namespace Uniars.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ApiClient Client;

        public const string CONFIG_FILE = "ClientConfig.json";

        public static BaseModel Config;

        public App()
        {
            InitializeComponent();

            Config = (BaseModel)JsonConfig.Load<BaseModel>(CONFIG_FILE);
            Client = new ApiClient(Config.ServerAddress);
        }
    }
}
