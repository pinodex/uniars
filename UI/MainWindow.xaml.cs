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

namespace Uniars.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected Dictionary<String, Page> NavigationPages = new Dictionary<String, Page>()
        {
            {"Overview", new Pages.Main.Overview()},
            {"Reports", new Pages.Main.Reports()},
            {"Booking", new Pages.Main.Booking()}
        };

        protected Boolean DeferNavigation = true;

        public MainWindow()
        {
            if (App.CurrentUser == null)
            {
                this.Close();
                return;
            }

            InitializeComponent();
            txtLoginUsername.Text = App.CurrentUser.Name;

            mainFrame.Content = NavigationPages.ElementAt(0).Value;

            DeferNavigation = false;
        }

        private void navigationClick(object sender, RoutedEventArgs e)
        {
            if (DeferNavigation)
            {
                return;
            }

            RadioButton button = (RadioButton)sender;
            String key = button.Content.ToString();

            if (NavigationPages.ContainsKey(key))
            {
                Page page;

                NavigationPages.TryGetValue(key, out page);
                mainFrame.Content = page;
            }

            txtPageTitle.Text = key;
        }

        private void btnLogoutClick(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            new LoginWindow().Show();

            this.Close();
        }
    }
}
