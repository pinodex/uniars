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
using Uniars.Client.Http;
using RestSharp;
using System.Net;
using System.ComponentModel;
using System.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace Uniars.Client.UI.Pages.Main
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Passengers : Page
    {
        private MetroWindow parentWindow;

        private BindingList<Passenger> passengerList = new BindingList<Passenger>();

        public Passengers(MetroWindow parentWindow)
        {
            InitializeComponent();

            this.parentWindow = parentWindow;
            this.passengersTable.DataContext = passengerList;

            this.LoadPassengerList();

            /*
            this.passengersTable.LoadingRow += (s, e) =>
            {
                string header = this.passengerListBox.Header.ToString();
                string updatedHeader = string.Format(header, DateTime.Now);

                this.passengerListBox.Header = updatedHeader;
            };
            */
        }

        private void LoadPassengerList()
        {
            ApiRequest request = new ApiRequest("passengers");

            App.Client.ExecuteAsync<PaginatedResult<Passenger>>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                PaginatedResult<Passenger> passengers = response.Data;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.passengerList.Repopulate<Passenger>(passengers.Data);
                }));
            });
        }

        private void SearchPassenger(string code)
        {
            ApiRequest request = new ApiRequest("passengers/" + code);

            App.Client.ExecuteAsync<Passenger>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        parentWindow.ShowMessageAsync("Search Passenger", "Cannot find passenger with the given code");
                    }));

                    return;
                }

                Passenger passenger = response.Data;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.passengerList.Repopulate<Passenger>(new List<Passenger>
                    {
                        passenger
                    });
                }));
            });
        }

        private void SearchPassenger(Dictionary<string, string> queries)
        {
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

                PaginatedResult<Passenger> passengers = response.Data;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.passengerList.Repopulate<Passenger>(passengers.Data);
                }));
            });
        }

        private void SearchCodeButtonClicked(object sender, RoutedEventArgs e)
        {
            string code = searchCodeText.Text;

            if (code == string.Empty)
            {
                parentWindow.ShowMessageAsync("Error", "Code cannot be empty.");
                return;
            }

            this.SearchPassenger(code);
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

            this.SearchPassenger(new Dictionary<string, string>
            {
                {"given_name", givenName},
                {"family_name", familyName},
                {"middle_name", middleName}
            });
        }

        private void ClearSearchButtonClicked(object sender, RoutedEventArgs e)
        {
            searchCodeText.Text = string.Empty;
            searchGivenNameText.Text = string.Empty;
            searchFamilyNameText.Text = string.Empty;
            searchMiddleNameText.Text = string.Empty;

            this.LoadPassengerList();
        }
    }
}
