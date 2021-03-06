﻿using System;
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
using System.Reflection;
using Uniars.Server.UI;
using Uniars.Server.Core.Config;
using Uniars.Shared.Database;
using Uniars.Shared.Foundation.Config;

namespace Uniars.Server
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static string ConnectionString;

        public const string CONFIG_FILE = "config\\server.json";

        public static BaseModel Config;

        public static Http.Server Server;

        public App()
        {
            try
            {
                Config = (BaseModel)JsonConfig.Load<BaseModel>(CONFIG_FILE);
            }
            catch
            {
                MessageBox.Show(string.Format("Unable to read configuration file from \"{0}{1}\"",
                    AppDomain.CurrentDomain.BaseDirectory, CONFIG_FILE));

                Environment.Exit(1);
            }

            ConnectionString = string.Format("Server={0};Uid={1};Password={2};Database={3}",
                App.Config.Database.Host,
                App.Config.Database.Username,
                App.Config.Database.Password,
                App.Config.Database.Name
            );

            Server = new Http.Server(App.Config.Server.Host, App.Config.Server.Port);

            InitializeComponent();
        }
    }
}
