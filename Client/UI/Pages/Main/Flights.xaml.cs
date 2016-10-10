using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using RestSharp;
using Uniars.Client.Core;
using Uniars.Client.Core.Collections;
using Uniars.Client.Http;
using Uniars.Client.UI.Pages.Flyout;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;
using Uniars.Shared.Foundation;

namespace Uniars.Client.UI.Pages.Main
{
    /// <summary>
    /// Interaction logic for Flights.xaml
    /// </summary>
    public partial class Flights : Page, IPollingList
    {
        public MainWindow parent;

        public FlightsModel model = new FlightsModel();

        public Dictionary<string, string> searchQuery;

        private bool disableAutoRefresh = false;

        private bool listLoadComplete = false;

        public Flights(MainWindow parent)
        {
            InitializeComponent();

            this.DataContext = model;
            this.parent = parent;

            model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == FlightsModel.P_CURRENT_PAGE && !model.IsLoadingActive)
                {
                    this.LoadList();
                }
            };

            model.FlightList.ListChanged += (sender, e) =>
            {
                model.LastUpdateTime = DateTime.Now;
            };

            model.Pages.ListChanged += (sender, e) =>
            {
                if (!this.listLoadComplete)
                {
                    return;
                }

                model.CurrentPage = 1;
            };

            model.EditorModel = this.CreateBlankModel();

            this.LoadList();
        }

        /// <summary>
        /// Create a blank Flight model
        /// </summary>
        /// <returns></returns>
        public Flight CreateBlankModel()
        {
            return new Flight();
        }

        /// <summary>
        /// Load list from server
        /// </summary>
        /// <param name="autoTriggered"></param>
        public void LoadList(bool autoTriggered = false)
        {
            if (autoTriggered && this.disableAutoRefresh)
            {
                return;
            }

            model.IsLoadingActive = true && !autoTriggered;

            string url = Url.FLIGHTS;

            if (this.searchQuery != null)
            {
                url = Url.FLIGHT_SEARCH;
            }

            ApiRequest request = new ApiRequest(url);
            request.AddParameter("page", model.CurrentPage);

            var responseAction = new Action<IRestResponse<PaginatedResult<Flight>>>(response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                PaginatedResult<Flight> result = response.Data;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    model.FlightList.Repopulate<Flight>(result.Data);
                    model.Pages.Repopulate<int>(result.GetPageList());

                    model.IsLoadingActive = false;

                    if (model.CurrentPage != result.Info.CurrentPage)
                    {
                        this.LoadList();
                    }
                }));

                this.listLoadComplete = true;
            });

            if (this.searchQuery == null)
            {
                App.Client.ExecuteAsync<PaginatedResult<Flight>>(request, responseAction);
                return;
            }

            ApiRequest.ExecuteParams<PaginatedResult<Flight>>(request, this.searchQuery, responseAction);
        }

        /// <summary>
        /// Set active tab
        /// </summary>
        /// <param name="index">Tab index</param>
        public void SetActiveTab(int index)
        {
            tabs.SelectedIndex = index;
        }

        /// <summary>
        /// Open flyout for flight
        /// </summary>
        /// <param name="model">Flight instance</param>
        public void OpenFlyout(Flight model)
        {
            if (model == null)
            {
                return;
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                parent.SetFlyoutContent("Flight", new FlightView(this, model));
                parent.OpenFlyout();
            }));
        }

        /// <summary>
        /// Reset editor
        /// </summary>
        public void ResetEditor()
        {
            model.EditorModel = this.CreateBlankModel();
            model.IsEditMode = false;
            model.IsEditorEnabled = true;

            this.SetActiveTab(0);
        }

        #region Events

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

        private void ClearSearchButtonClicked(object sender, RoutedEventArgs e)
        {
            searchAirlineText.Text = string.Empty;
            searchSourceText.Text = string.Empty;
            searchDestinationText.Text = string.Empty;

            this.searchQuery = null;
            this.disableAutoRefresh = false;

            this.LoadList();
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            string flight = searchAirlineText.Text;
            string source = searchSourceText.Text;
            string destination = searchDestinationText.Text;

            model.IsLoadingActive = true;
            this.disableAutoRefresh = true;

            this.searchQuery = new Dictionary<string, string>
            {
                {"flight", flight},
                {"source", source},
                {"destination", destination}
            };

            this.LoadList();
        }

        private void ListRowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = ItemsControl.ContainerFromElement(
                sender as DataGrid, e.OriginalSource as DependencyObject) as DataGridRow;

            if (row == null)
            {
                return;
            }

            this.OpenFlyout(this.table.SelectedItem as Flight);

            e.Handled = true;
        }

        private void ListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.OpenFlyout(this.table.SelectedItem as Flight);

                e.Handled = true;
            }
        }

        private void SearchTextBoxButtonClicked(object sender, MouseButtonEventArgs e)
        {
            TextBox button = sender as TextBox;
            string mode = button.Tag.ToString();

            Flight flight = model.EditorModel.Clone();

            switch (mode)
            {
                case "AirlineSearch":
                    Airlines airlinesPage = new Airlines(this.parent);
                    airlinesPage.EnablePicker(result =>
                    {
                        flight.Airline = result;
                        model.EditorModel = flight;

                        parent.CloseBigFlyout();
                    });

                    parent.SetBigFlyoutContent("Handler Airline", airlinesPage);
                    break;

                case "SourceSearch":
                    Airports sourceAirportPage = new Airports(this.parent);
                    sourceAirportPage.EnablePicker(result =>
                    {
                        flight.Source = result;
                        model.EditorModel = flight;

                        parent.CloseBigFlyout();
                    });

                    parent.SetBigFlyoutContent("Source Airport", sourceAirportPage);

                    break;

                case "DestinationSearch":
                    Airports destAirportPage = new Airports(this.parent);
                    destAirportPage.EnablePicker(result =>
                    {
                        flight.Destination = result;
                        model.EditorModel = flight;

                        parent.CloseBigFlyout();
                    });

                    parent.SetBigFlyoutContent("Destination Airport", destAirportPage);

                    break;
            }

            parent.OpenBigFlyout();
        }

        private void EditorDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            string message = string.Format("Are you sure you want to delete flight \"{0}\"? This action is irreversible.",
                model.EditorModel.Code);

            parent.ShowMessageAsync("Delete Flight", message, MessageDialogStyle.AffirmativeAndNegative).ContinueWith(task =>
            {
                if (task.Result == MessageDialogResult.Negative)
                {
                    return;
                }

                this.Dispatcher.Invoke(new Action(() => model.IsEditorEnabled = false));

                ApiRequest request = new ApiRequest(Url.FLIGHTS + "/" + model.EditorModel.Id, Method.DELETE);

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

            string url = Url.FLIGHTS;
            Method method = Method.POST;

            if (model.EditorModel.Id != 0)
            {
                url += "/" + model.EditorModel.Id;
                method = Method.PUT;
            }

            model.EditorModel.AirlineId = model.EditorModel.Airline.Id;
            model.EditorModel.SourceId = model.EditorModel.Source.Id;
            model.EditorModel.DestinationId = model.EditorModel.Destination.Id;

            ApiRequest request = new ApiRequest(url, method);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(model.EditorModel);

            App.Client.ExecuteAsync<Flight>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        parent.ShowMessageAsync("Error", "Unable to save flight formation.");
                        model.IsEditorEnabled = true;
                    }));

                    return;
                }

                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.LoadList();
                    this.OpenFlyout(response.Data);

                    this.ResetEditor();
                }));
            });
        }

        #endregion
    }
}
