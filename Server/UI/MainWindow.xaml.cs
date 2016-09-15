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

namespace Uniars.Server.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            menuItemHelpVersion.Header = string.Format(menuItemHelpVersion.Header.ToString(), App.Version);
            gridServerStatusStopped.Visibility = Visibility.Visible;

            App.Server.OnStart += new Http.ServerStartedEvent((object sender, EventArgs e) => {
                gridServerStatusStarting.Visibility = Visibility.Hidden;
                gridServerStatusStopped.Visibility = Visibility.Hidden;
                gridServerStatus.Visibility = Visibility.Visible;

                txtStatusListening.Text = string.Format(txtStatusListening.Text, App.Server.Host, App.Server.Port);
                txtStatusConnections.Text = string.Format(txtStatusConnections.Text, 0);
            });

            App.Server.OnStarting += new Http.ServerStartingEvent((object sender, EventArgs e) => {
                gridServerStatusStopped.Visibility = Visibility.Hidden;
                gridServerStatus.Visibility = Visibility.Hidden;
                gridServerStatusStarting.Visibility = Visibility.Visible;
            });

            App.Server.OnStop += new Http.ServerStoppedEvent((object sender, EventArgs e) => {
                gridServerStatus.Visibility = Visibility.Hidden;
                gridServerStatusStarting.Visibility = Visibility.Hidden;
                gridServerStatusStopped.Visibility = Visibility.Visible;
            });
        }

        private void SettingsClicked(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }

        private void ServerStartClicked(object sender, RoutedEventArgs e)
        {
            App.Server.Start();
        }

        private void ServerStopClicked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmDialog = MessageBox.Show("Stop server? This will disconnect all clients",
                "Confirm Action", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (confirmDialog == MessageBoxResult.Cancel)
            {
                return;
            }

            App.Server.Stop();
        }

        private void ServerRestartClicked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmDialog = MessageBox.Show("Restart server? This will disconnect all clients",
                "Confirm Action", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (confirmDialog == MessageBoxResult.Cancel)
            {
                return;
            }

            App.Server.Restart();
        }

        private void AboutClicked(object sender, RoutedEventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
