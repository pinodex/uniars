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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace Uniars.Client.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        protected Dictionary<string, Page> map = new Dictionary<string, Page>
        {
            {"Overview", new Pages.Main.Overview()},
            {"Flyers", new Pages.Main.Flyers()},
        };

        private bool deferMenuSelectionChange = true;

        public MainWindow()
        {
            InitializeComponent();

            if (App.Client.CurrentUser == null)
            {
                this.Close();
                return;
            }

            txtUsername.Text = App.Client.CurrentUser.Name;

            this.Loaded += (s, e) =>
            {
                mainFrame.Content = map.ElementAt(0).Value;
                this.deferMenuSelectionChange = false;
            };

            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.System && e.SystemKey == Key.F4)
                {
                    this.btnSignOut_Click(s, e);
                    e.Handled = true;
                }
            };
        }

        private void Logout()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.IsEnabled = false;
                App.Client.Logout();
                this.Close();
            }));
        }

        private void menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.deferMenuSelectionChange)
            {
                return;
            }

            ListBox menu = (ListBox)sender;

            if (menu.SelectedItem == null)
            {
                return;
            }

            ListBoxItem item = menu.SelectedItem as ListBoxItem;

            if (item.Tag == null)
            {
                return;
            }

            string key = item.Tag.ToString();

            Page content;

            if (map.ContainsKey(key))
            {
                map.TryGetValue(key, out content);

                mainFrame.Content = content;
            }
        }

        private void btnAccountSettings_Click(object sender, RoutedEventArgs e)
        {
            this.ShowMessageAsync("Coming soon", "Feature not yet ready.");
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            MetroDialogSettings settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Sign out"
            };

            Task<MessageDialogResult> messageTask = this.ShowMessageAsync("Sign out", "Are you sure you want to sign out?", MessageDialogStyle.AffirmativeAndNegative, settings);
            
            messageTask.ContinueWith(task =>
            {
                if (task.Result == MessageDialogResult.Affirmative)
                {
                    this.Logout();
                }
            });
        }
    }
}
