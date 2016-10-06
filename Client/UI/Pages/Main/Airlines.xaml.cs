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
    /// Interaction logic for Airlines.xaml
    /// </summary>
    public partial class Airlines : Page
    {
        public MainWindow parent;

        public AirlinesModel model = new AirlinesModel();

        public Dictionary<string, string> searchQuery;

        public bool deferAutoRefresh = false;

        public Airlines(MainWindow parent)
        {
            InitializeComponent();

            this.DataContext = model;
            this.parent = parent;

            model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == AirlinesModel.P_CURRENT_PAGE && !model.IsLoadingActive)
                {
                    this.LoadAirlineList();
                }
            };

            model.AirlineList.ListChanged += (sender, e) =>
            {
                model.LastUpdateTime = DateTime.Now;
            };

            this.LoadAirlineList();
            this.LoadCountries();

            DispatcherTimer listTimer = new DispatcherTimer();

            listTimer.Tick += (sender, e) =>
            {
                if (!deferAutoRefresh)
                {
                    this.LoadAirlineList(true);
                }
            };

            listTimer.Interval = new TimeSpan(0, 0, 5);
            listTimer.Start();
        }

        public void LoadAirlineList(bool autoTriggered = false)
        {
            model.IsLoadingActive = true && !autoTriggered;

            string url = Url.AIRLINES;

            if (this.searchQuery != null)
            {
                url = Url.AIRLINE_SEARCH;
            }

            ApiRequest request = new ApiRequest(url);
            request.AddParameter("page", model.CurrentPage);

            var responseAction = new Action<IRestResponse<PaginatedResult<Airline>>>(response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                PaginatedResult<Airline> result = response.Data;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    model.AirlineList.Repopulate<Airline>(result.Data);

                    if (model.Pages.Count() == 0)
                    {
                        model.Pages.Repopulate<int>(result.GetPageList());
                    }

                    model.IsLoadingActive = false;
                }));
            });

            if (this.searchQuery == null)
            {
                App.Client.ExecuteAsync<PaginatedResult<Airline>>(request, responseAction);
                return;
            }

            ApiRequest.Search<PaginatedResult<Airline>>(request, this.searchQuery, responseAction);
        }

        public void LoadCountries()
        {
            ApiRequest request = new ApiRequest(Url.COUNTRIES_ALL);

            App.Client.ExecuteAsync<List<Country>>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                this.Dispatcher.Invoke(new Action(() => model.CountryList = response.Data));
            });
        }

        private void SearchTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.SearchButtonClicked(sender, e);
            }

            if (e.Key == Key.Escape)
            {
                this.ClearSearchButtonClicked(sender, e);
            }
        }

        public void ClearSearchButtonClicked(object sender, RoutedEventArgs e)
        {
            searchNameText.Text = string.Empty;
            searchCallsignText.Text = string.Empty;
            searchCountryText.SelectedItem = null;

            this.searchQuery = null;
            this.deferAutoRefresh = false;

            model.Pages.Clear();

            this.LoadAirlineList();
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            string name = searchNameText.Text;
            string callsign = searchCallsignText.Text;
            string country = string.Empty;

            if (searchCountryText.SelectedItem != null)
            {
                country = (searchCountryText.SelectedItem as Country).Name;
            }

            model.IsLoadingActive = true;
            this.deferAutoRefresh = true;

            this.searchQuery = new Dictionary<string, string>
            {
                {"name", name},
                {"callsign", callsign},
                {"country", country}
            };

            this.LoadAirlineList();
        }
    }
}
