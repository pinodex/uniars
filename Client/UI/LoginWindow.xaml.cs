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
using Uniars.Client.Http;
using Uniars.Shared.Database.Entity;
using MahApps.Metro.Controls;
using System.Windows.Media.Animation;

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

        private void btnLoginClick(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == "/exit")
            {
                this.Close();
                return;
            }

            txtLoginError.Visibility = Visibility.Hidden;
            btnLogin.IsEnabled = false;
            windowLogin.IsEnabled = false;

            App.Client.LoginAsync(txtUsername.Text, txtPassword.Password, (isOk, user) =>
            {
                if (!isOk || user == null)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.ResetLoginForm();
                        this.AlertInvalidLogin();
                    }));
                    return;
                }

                this.Dispatcher.Invoke(new Action(() =>
                    {
                        new MainWindow().Show();
                        this.Close();
                    }
                ));
            });
        }

        protected void AlertInvalidLogin()
        {
            txtLoginError.Visibility = Visibility.Visible;

            errorBox.Opacity = 1;

            DoubleAnimation errorBoxAnimation = new DoubleAnimation
            {
                To = 0,
                BeginTime = TimeSpan.FromSeconds(1),
                Duration = TimeSpan.FromSeconds(5),
                FillBehavior = FillBehavior.Stop
            };

            DoubleAnimation windowAnimation = new DoubleAnimation
            {

            };

            errorBoxAnimation.Completed += (s, a) => errorBox.Opacity = 0;
            errorBox.BeginAnimation(UIElement.OpacityProperty, errorBoxAnimation);
        }

        protected void ResetLoginForm()
        {
            txtPassword.Password = "";
            
            txtLoginError.Visibility = Visibility.Hidden;
            btnLogin.IsEnabled = true;
            windowLogin.IsEnabled = true;

            txtUsername.Focus();
        }

        private void windowLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }
    }
}
