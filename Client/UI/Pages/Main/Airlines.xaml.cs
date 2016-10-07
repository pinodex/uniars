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
                    this.LoadList();
                }
            };

            model.AirlineList.ListChanged += (sender, e) =>
            {
                model.LastUpdateTime = DateTime.Now;
            };

            model.EditorModel = this.CreateBlankModel();

            this.LoadList();
            this.LoadCountries();

            DispatcherTimer listTimer = new DispatcherTimer();

            listTimer.Tick += (sender, e) =>
            {
                if (!deferAutoRefresh)
                {
                    this.LoadList(true);
                }
            };

            listTimer.Interval = new TimeSpan(0, 0, 5);
            listTimer.Start();
        }

        public Airline CreateBlankModel()
        {
            return new Airline();
        }

        public void LoadList(bool autoTriggered = false)
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
                    model.Pages.Repopulate<int>(result.GetPageList());

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

            this.LoadList();
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

            this.LoadList();
        }

        public void SetActiveTab(int index)
        {
            tabs.SelectedIndex = index;
        }

        public void OpenFlyout(Airline model)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                parent.SetFlyoutContent("Airline", new AirlineView(this, model));
                parent.OpenFlyout();
            }));
        }

        public void ResetEditor()
        {
            model.EditorModel = this.CreateBlankModel();
            model.IsEditMode = false;
            model.IsEditorEnabled = true;

            this.SetActiveTab(0);
        }

        private void ListRowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = ItemsControl.ContainerFromElement(
                sender as DataGrid, e.OriginalSource as DependencyObject) as DataGridRow;

            if (row == null)
            {
                return;
            }

            this.OpenFlyout(this.passengersTable.SelectedItem as Airline);

            e.Handled = true;
        }

        private void ListKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void EditorDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            string message = string.Format("Are you sure you want to delete \"{0}\" from airline list? This action is irreversible.",
                model.EditorModel.Name);

            parent.ShowMessageAsync("Delete Airline", message, MessageDialogStyle.AffirmativeAndNegative).ContinueWith(task =>
            {
                if (task.Result == MessageDialogResult.Negative)
                {
                    return;
                }

                this.Dispatcher.Invoke(new Action(() => model.IsEditorEnabled = false));

                ApiRequest request = new ApiRequest(Url.AIRLINES + "/" + model.EditorModel.Id, Method.DELETE);

                App.Client.ExecuteAsync(request, response =>
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.LoadList();
                        this.ResetEditor();
                    }));
                });
            });
        }

        private void EditorClearButtonClicked(object sender, RoutedEventArgs e)
        {
            model.EditorModel = this.CreateBlankModel();
        }

        private void EditorDiscardButtonClicked(object sender, RoutedEventArgs e)
        {
            this.ResetEditor();
        }

        private void EditorSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            model.IsEditorEnabled = false;

            string url = Url.AIRLINES;
            Method method = Method.POST;

            if (model.EditorModel.Id != 0)
            {
                url += "/" + model.EditorModel.Id;
                method = Method.PUT;
            }

            ApiRequest request = new ApiRequest(url, method);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(model.EditorModel);

            App.Client.ExecuteAsync<Airline>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    this.Dispatcher.Invoke(new Action(() => parent.ShowMessageAsync("Error", "Unable to save airline formation.")));

                    return;
                }

                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.LoadList();
                    this.OpenFlyout(response.Data);
                    this.SetActiveTab(0);

                    model.EditorModel = this.CreateBlankModel();
                    model.IsEditorEnabled = true;
                    model.IsEditMode = false;
                }));
            });
        }
    }
}
