using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
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
using SWF = System.Windows.Forms;
using MySql.Data.MySqlClient;
using Uniars.Shared.Foundation;
using Uniars.Shared.Foundation.Config;

namespace Uniars.Server.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            LoadValues();
        }


        /// <summary>
        /// Load values to text boxes from config
        /// </summary>
        private void LoadValues()
        {
            txtServerIpAddress.Text = App.Config.Server.Host;
            txtServerPort.Text = App.Config.Server.Port.ToString();
            
            txtDatabaseHost.Text = App.Config.Database.Host;
            txtDatabaseUsername.Text = App.Config.Database.Username;
            txtDatabasePassword.Password = App.Config.Database.Password;
            txtDatabaseName.Text = App.Config.Database.Name;

            txtSecurityCertificateFile.Text = App.Config.CertificateFile;
        }

        /// <summary>
        /// Save values to config file from text boxes
        /// </summary>
        private void SaveChanges()
        {
            App.Config.Server.Host = txtServerIpAddress.Text;
            App.Config.Server.Port = uint.Parse(txtServerPort.Text);

            App.Config.Database.Host = txtDatabaseHost.Text;
            App.Config.Database.Username = txtDatabaseUsername.Text;
            App.Config.Database.Password = txtDatabasePassword.Password;
            App.Config.Database.Name = txtDatabaseName.Text;

            App.Config.CertificateFile = txtSecurityCertificateFile.Text;

            JsonConfig.Write(App.CONFIG_FILE, App.Config);
        }

        private void TestBindingClicked(object sender, RoutedEventArgs e)
        {
            if (App.Server.IsRunning)
            {
                MessageBox.Show("Cannot test host binding. Server is currently running", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Asterisk);

                return;
            }

            uint port;
            Http.Server testServer = default(Http.Server);

            try
            {
                port = uint.Parse(txtServerPort.Text);
                testServer = new Http.Server(txtServerIpAddress.Text, port);

                testServer.Start();

                MessageBox.Show("Host and port available", "Test Binding", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Host and port unavailable", "Test Binding", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                if (testServer != null)
                {
                    testServer.Stop();
                }
            }
        }

        private void TestDatabaseConnectionClicked(object sender, RoutedEventArgs e)
        {
            string connectionString = string.Format(
                "Server={0};Uid={1};Pwd={2};Database={3}",
                
                txtDatabaseHost.Text,
                txtDatabaseUsername.Text,
                txtDatabasePassword.Password,
                txtDatabaseName.Text
            );

            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();

                MessageBox.Show("Database connection succeeded\n\nServer version: " + conn.ServerVersion,
                    "Test Database Connection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Database connection failed\n\n" + ex.Message,
                    "Test Database Connection", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void BrowseCertificateClicked(object sender, RoutedEventArgs e)
        {
            SWF.OpenFileDialog dialog = new SWF.OpenFileDialog();

            dialog.Filter = "Certificate File|*.crt";

            SWF.DialogResult result = dialog.ShowDialog();

            if (result == SWF.DialogResult.Cancel)
            {
                return;
            }

            txtSecurityCertificateFile.Text = dialog.FileName;
            App.Config.CertificateFile = dialog.FileName;
        }

        private void OkClicked(object sender, RoutedEventArgs e)
        {
            SaveChanges();

            MessageBox.Show("Changes will only take effect after restarting the application",
                "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
