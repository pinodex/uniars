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
using System.Net;
using System.Xml;
using Uniars.Client.Http;
using Uniars.Shared.Database.Entity;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Media.Animation;

namespace Uniars.Client.UI
{
    public partial class LoginWindow : MetroWindow
    {
        public LoginWindow()
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                txtUsername.Focus();

                App.Client.TestConnect(response =>
                {
                    if (response.ResponseStatus != RestSharp.ResponseStatus.Completed)
                    {
                        ShowConnectErrorDialog();
                    }
                });
            };

            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.System && e.SystemKey == Key.F4)
                {
                    e.Handled = true;
                }
            };

#if DEBUG
            txtUsername.Text = "admin";
            txtPassword.Password = "admin";

            btnLoginClick(null, null);
#endif
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

            App.Client.LoginAsync(txtUsername.Text, txtPassword.Password, (response) =>
            {
                if (response.ResponseStatus != RestSharp.ResponseStatus.Completed)
                {
                    this.ResetLoginForm();
                    this.ShowConnectErrorDialog();

                    return;
                }
                
                if (response.StatusCode != HttpStatusCode.OK || response.Data == null)
                {
                    this.ResetLoginForm();
                    this.AlertInvalidLogin();

                    return;
                }

                this.ResetLoginForm(true);

                this.Dispatcher.Invoke(new Action(() =>
                    {
                        Window mainWindow = new MainWindow();
                        mainWindow.Show();
                    }
                ));
            });
        }

        protected void ShowConnectErrorDialog()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.ShowMessageAsync("Error", "Cannot establish connection to server");
            }));
        }

        protected void AlertInvalidLogin()
        {
            this.Dispatcher.Invoke(new Action(() =>
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

                errorBoxAnimation.Completed += (s, a) => errorBox.Opacity = 0;
                errorBox.BeginAnimation(UIElement.OpacityProperty, errorBoxAnimation);
            }));
        }

        protected void ResetLoginForm(bool all = false)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (all)
                {
                    txtUsername.Text = "";
                }

                txtPassword.Password = "";
            
                txtLoginError.Visibility = Visibility.Hidden;
                btnLogin.IsEnabled = true;
                windowLogin.IsEnabled = true;

                txtUsername.Focus();
            }));
        }
    }
}
