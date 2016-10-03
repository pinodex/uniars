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
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;
using Uniars.Client.Core.Collections;
using Uniars.Client.UI.Pages.Flyout;
using Uniars.Client.Http;
using RestSharp;
using System.Net;
using System.ComponentModel;
using System.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Uniars.Client.UI.Pages.Main
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Passengers : Page
    {
        private MainWindow parentWindow;

        private PassengersModel model = new PassengersModel
        {
            IsLoadingActive = false
        };

        private bool deferAutoRefresh = false;

        public Passengers(MainWindow parentWindow)
        {
            InitializeComponent();

            this.DataContext = model;
            this.parentWindow = parentWindow;

            model.PassengerList.ListChanged += (sender, e) =>
            {
                model.LastUpdateTime = DateTime.Now;
            };

            this.LoadPassengerList();

            DispatcherTimer listTimer = new DispatcherTimer();

            listTimer.Tick += (sender, e) =>
            {
                if (!deferAutoRefresh)
                {
                    this.LoadPassengerList();
                }
            };

            listTimer.Interval = new TimeSpan(0, 0, 5);
            listTimer.Start();
        }

        protected void LoadPassengerList()
        {
            ApiRequest request = new ApiRequest("passengers");

            App.Client.ExecuteAsync<PaginatedResult<Passenger>>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                PaginatedResult<Passenger> passengers = response.Data;

                this.Dispatcher.Invoke(new Action(() => model.PassengerList.Repopulate<Passenger>(passengers.Data)));
            });
        }

        protected void OpenPassengerFlyout(Passenger passenger)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                parentWindow.SetFlyoutContent("Passenger", new PassengerView(passenger));
                parentWindow.OpenFlyout();
            }));
        }

        private void SearchPassenger(string code, Action<Passenger> result)
        {
            ApiRequest request = new ApiRequest("passengers/" + code);

            App.Client.ExecuteAsync<Passenger>(request, response => result(response.Data));
        }

        private void SearchPassenger(Dictionary<string, string> queries, Action<PaginatedResult<Passenger>> result)
        {
            model.IsLoadingActive = true;

            ApiRequest request = new ApiRequest("passengers/search");

            foreach (KeyValuePair<string, string> query in queries)
            {
                request.AddQueryParameter(query.Key, query.Value);
            }

            App.Client.ExecuteAsync<PaginatedResult<Passenger>>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                result(response.Data);
            });
        }

        private void PassengerListRowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = ItemsControl.ContainerFromElement(
                sender as DataGrid, e.OriginalSource as DependencyObject) as DataGridRow;

            if (row == null)
            {
                return;
            }

            this.OpenPassengerFlyout(this.passengersTable.SelectedItem as Passenger);

            e.Handled = true;
        }

        private void PassengerListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.OpenPassengerFlyout(this.passengersTable.SelectedItem as Passenger);

                e.Handled = true;
            }
        }

        private void SearchTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            string tag = (sender as TextBox).Tag.ToString();

            if (e.Key == Key.Enter)
            {
                switch (tag)
                {
                    case "CodeSearch":
                        this.SearchCodeButtonClicked(null, null);
                        break;

                    case "NameSearch":
                        this.SearchNameButtonClicked(null, null);
                        break;
                }
            }

            if (e.Key == Key.Escape)
            {
                switch (tag)
                {
                    case "CodeSearch":
                        searchCodeText.Text = string.Empty;

                        break;

                    case "NameSearch":
                        this.ClearSearchButtonClicked(null, null);

                        break;
                }
            }
        }

        private void SearchCodeButtonClicked(object sender, RoutedEventArgs e)
        {
            string code = searchCodeText.Text;

            if (code == string.Empty)
            {
                parentWindow.ShowMessageAsync("Error", "Code cannot be empty.");
                return;
            }

            model.IsCodeSearchEnabled = false;

            this.SearchPassenger(code, passenger =>
            {
                this.OpenPassengerFlyout(passenger);

                this.Dispatcher.Invoke(new Action(() =>
                {
                    model.IsCodeSearchEnabled = true;
                    searchCodeText.Text = string.Empty;
                }));
            });
        }

        private void SearchNameButtonClicked(object sender, RoutedEventArgs e)
        {
            string givenName = searchGivenNameText.Text;
            string familyName = searchFamilyNameText.Text;
            string middleName = searchMiddleNameText.Text;

            if (givenName == string.Empty && familyName == string.Empty && middleName == string.Empty)
            {
                parentWindow.ShowMessageAsync("Error", "Please fill at least one search criteria.");
                return;
            }

            model.IsNameSearchEnabled = false;
            this.deferAutoRefresh = true;

            Dictionary<string, string> query = new Dictionary<string, string>
            {
                {"given_name", givenName},
                {"family_name", familyName},
                {"middle_name", middleName}
            };

            this.SearchPassenger(query, result => this.Dispatcher.Invoke(new Action(() =>
                {
                    model.PassengerList.Repopulate<Passenger>(result.Data);
                    
                    model.IsLoadingActive = false;
                    model.IsNameSearchEnabled = true;
                }))
            );
        }

        private void ClearSearchButtonClicked(object sender, RoutedEventArgs e)
        {
            searchGivenNameText.Text = string.Empty;
            searchFamilyNameText.Text = string.Empty;
            searchMiddleNameText.Text = string.Empty;

            this.deferAutoRefresh = false;
            this.LoadPassengerList();
        }
    }
}
