﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Uniars.Client.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        protected Dictionary<string, Page> map;

        private bool deferMenuSelectionChange = true;

        private MainWindowModel model = new MainWindowModel();

        public MainWindow()
        {
            InitializeComponent();

            if (App.Client.CurrentUser == null)
            {
                this.Close();
                return;
            }

            this.DataContext = model;

            DispatcherTimer clock = new DispatcherTimer();
            
            clock.Tick += (sender, e) => model.CurrentDateTime = DateTime.Now;
            clock.Interval = new TimeSpan(0, 0, 1);
            clock.Start();

            model.Username = App.Client.CurrentUser.Name;

            map = new Dictionary<string, Page>
            {
                {"Overview", new Pages.Main.Overview()},
                {"Passengers", new Pages.Main.Passengers(this)},
                {"Airlines", new Pages.Main.Airlines(this)},
            };

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

#if DEBUG
                if (e.Key == Key.F12)
                {
                    Application.Current.Shutdown();
                }
#endif
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

        public void SetFlyoutContent(string header, Page content)
        {
            this.mainFlyout.Header = header;
            this.mainFlyoutFrame.Content = content;
        }

        public void OpenFlyout()
        {
            this.mainFlyout.IsOpen = true;
        }

        public void CloseFlyout()
        {
            this.mainFlyout.IsOpen = false;
        }

        #region Events

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

        #endregion
    }
}
