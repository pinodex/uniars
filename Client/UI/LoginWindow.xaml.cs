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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Uniars.Client.UI
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            txtUsername.Focus();
        }

        public void menuTabCliked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            switch (button.Tag.ToString())
            {
                case "tabBookingManager":
                    borderBookingManager.BorderBrush = Brushes.Silver;
                    borderBookingManager.Background = Brushes.White;

                    borderSystemAdministration.BorderBrush = null;
                    borderSystemAdministration.Background = null;

                    break;

                case "tabSystemAdministration":
                    borderSystemAdministration.BorderBrush = Brushes.Silver;
                    borderSystemAdministration.Background = Brushes.White;

                    borderBookingManager.BorderBrush = null;
                    borderBookingManager.Background = null;

                    break;
            }

            tbkLoginAction.Text = button.Content.ToString();
        }

        private void btnLoginClick(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == string.Empty || txtPassword.Password == string.Empty)
            {
                txtLoginError.Visibility = Visibility.Visible;

                return;
            }

            txtLoginError.Visibility = Visibility.Hidden;
            btnLogin.Content = "Logging in";
            windowLogin.IsEnabled = false;

            new MainWindow().Show();
            this.Close();
        }
    }
}
