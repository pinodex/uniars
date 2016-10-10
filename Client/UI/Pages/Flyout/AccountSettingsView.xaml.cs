using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using RestSharp;
using Uniars.Client.Http;
using Uniars.Shared.Database.Entity;

namespace Uniars.Client.UI.Pages.Flyout
{
    /// <summary>
    /// Interaction logic for Passenger.xaml
    /// </summary>
    public partial class AccountSettingsView : Page
    {
        private MainWindow parent;

        private MainWindowModel model;

        public AccountSettingsView(MainWindow parent, MainWindowModel model)
        {
            InitializeComponent();

            this.DataContext = model;
            
            this.parent = parent;
            this.model = model;
        }

        #region Events

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            string currentPassword = txtCurrentPassword.Password;
            string newPassword = txtPassword.Password;
            string newPasswordConfirm = txtPasswordConfirm.Password;

            if (newPassword != newPasswordConfirm)
            {
                parent.ShowMessageAsync("Error", "Passwords do not match.");
                return;
            }

            if (newPassword.Length < 8)
            {
                parent.ShowMessageAsync("Error", "Passwords should have at least 8 characters.");
                return;
            }

            if (newPassword == string.Empty)
            {
                parent.ShowMessageAsync("Settings", "No changes made.");
                parent.CloseFlyout();
                return;
            }

            this.model.IsSettingsEnabled = false;

            ApiRequest request = new ApiRequest(Url.SETTINGS_PASSWORD, Method.PUT);
            
            request.AddParameter("current_password", currentPassword);
            request.AddParameter("new_password", newPassword);

            App.Client.ExecuteAsync<User>(request, response =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.model.IsSettingsEnabled = true;

                    if (response.StatusCode == HttpStatusCode.NotAcceptable)
                    {
                        parent.ShowMessageAsync("Error", "Invalid current password.");
                        return;
                    }

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        parent.ShowMessageAsync("Error", "Unable to save settings.");
                        return;
                    }

                    parent.ShowMessageAsync("Settings", "Account settings saved");
                    parent.CloseFlyout();
                }));
            });
        }

        #endregion
    }
}
