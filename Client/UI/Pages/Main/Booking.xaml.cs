using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using RestSharp;
using Uniars.Client.Core.Collections;
using Uniars.Client.Http;
using Uniars.Client.UI.Pages.Flyout;
using Uniars.Shared.Model;
using Uniars.Shared.Database;
using Uniars.Shared.Foundation;
using Uniars.Shared.Database.Entity;
using Uniars.Client.Core;
using System.Collections;

namespace Uniars.Client.UI.Pages.Main
{
    /// <summary>
    /// Interaction logic for Booking.xaml
    /// </summary>
    public partial class Booking : Page, IPollingList
    {
        public MainWindow parent;

        public BookingModel model = new BookingModel();

        public Dictionary<string, string> searchQuery;

        private bool disableAutoRefresh = false;

        private bool listLoadComplete = false;

        public Booking(MainWindow parent)
        {
            InitializeComponent();

            this.DataContext = model;
            this.parent = parent;

            model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == BookingModel.P_CURRENT_PAGE && !model.IsLoadingActive)
                {
                    this.LoadList();
                }
            };

            model.BookingList.ListChanged += (sender, e) =>
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
        /// Create a blank Booking model
        /// </summary>
        /// <returns></returns>
        public Book CreateBlankModel()
        {
            return new Book();
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

            ApiRequest request = new ApiRequest(Url.BOOKINGS);
            request.AddParameter("page", model.CurrentPage);

            var responseAction = new Action<IRestResponse<PaginatedResult<Book>>>(response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                PaginatedResult<Book> result = response.Data;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    model.BookingList.Repopulate<Book>(result.Data);
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
                App.Client.ExecuteAsync<PaginatedResult<Book>>(request, responseAction);
                return;
            }

            ApiRequest.ExecuteParams<PaginatedResult<Book>>(request, this.searchQuery, responseAction);
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
        /// Open flyout for booking
        /// </summary>
        /// <param name="model">Booking instance</param>
        public void OpenFlyout(Book model)
        {
            if (model == null)
            {
                return;
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                parent.SetFlyoutContent("Booking", new BookingView(this, model));
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

        /// <summary>
        /// Search by passenger object
        /// </summary>
        /// <param name="passenger">Passenger object</param>
        public void SearchByPassenger(Passenger passenger)
        {
            this.ClearSearchButtonClicked(this, null);

            searchPassengerText.Text = passenger.DisplayName;
            searchPassengerIdText.Text = passenger.Id.ToString();

            this.SearchButtonClicked(this, null);
        }

        #region Events

        private void SearchPassengerTextButtonClicked(object sender, MouseButtonEventArgs e)
        {
            Passengers passengersPage = new Passengers(this.parent);

            passengersPage.EnablePicker((Passenger result) =>
            {
                searchPassengerText.Text = result.DisplayName;
                searchPassengerIdText.Text = result.Id.ToString();

                parent.CloseBigFlyout();
            });

            parent.SetBigFlyoutContent("Passenger", passengersPage);
            parent.OpenBigFlyout();
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

        private void ClearSearchButtonClicked(object sender, RoutedEventArgs e)
        {
            searchFlightCodeText.Text = string.Empty;
            searchAirlineText.Text = string.Empty;
            searchSourceText.Text = string.Empty;
            searchDestinationText.Text = string.Empty;
            searchPassengerText.Text = string.Empty;
            searchPassengerIdText.Text = string.Empty;

            this.searchQuery = null;
            this.disableAutoRefresh = false;

            this.LoadList();
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            string flight = searchFlightCodeText.Text;
            string airline = searchAirlineText.Text;
            string source = searchSourceText.Text;
            string destination = searchDestinationText.Text;
            string passengerId = searchPassengerIdText.Text;

            model.IsLoadingActive = true;
            this.disableAutoRefresh = true;

            this.searchQuery = new Dictionary<string, string>
            {
                {"flight", flight},
                {"airline", airline},
                {"source", source},
                {"destination", destination},
                {"passenger_id", passengerId}
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

            this.OpenFlyout(this.table.SelectedItem as Book);

            e.Handled = true;
        }

        private void ListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.OpenFlyout(this.table.SelectedItem as Book);

                e.Handled = true;
            }
        }

        private void SearchTextBoxButtonClicked(object sender, MouseButtonEventArgs e)
        {
            TextBox button = sender as TextBox;
            string mode = button.Tag.ToString();

            Book book = model.EditorModel.Clone();

            switch (mode)
            {
                case "FlightSearch":
                    Flights flightsPage = new Flights(this.parent);
                    
                    flightsPage.EnablePicker(result =>
                    {
                        book.Flight = result;
                        model.EditorModel = book;

                        parent.CloseBigFlyout();
                    });

                    parent.SetBigFlyoutContent("Flight", flightsPage);
                    break;
            }

            parent.OpenBigFlyout();
        }

        private void EditorAddPassengerClicked(object sender, RoutedEventArgs e)
        {
            Passengers passengersPage = new Passengers(this.parent);
            Book book= model.EditorModel.Clone();

            if (book.Passengers == null)
            {
                book.Passengers = new List<Passenger>();
            }
            
            passengersPage.EnablePicker(result =>
            {
                result.RemoveAll(r => book.Passengers.Any(p => p.Id == r.Id));
                book.Passengers.AddRange(result);

                model.EditorModel = book;

                parent.CloseBigFlyout();
            });

            parent.SetBigFlyoutContent("Passengers", passengersPage);
            parent.OpenBigFlyout();
        }

        private void EditorDeletePassengerClicked(object sender, RoutedEventArgs e)
        {
            Passengers passengersPage = new Passengers(this.parent);
            Book book = model.EditorModel.Clone();

            if (book.Passengers == null)
            {
                book.Passengers = new List<Passenger>();
            }

            List<Passenger> selectedPassengers = tableSelected.SelectedItems.OfType<Passenger>().ToList();

            book.Passengers.RemoveAll(m => selectedPassengers.Any(p => p.Id == m.Id));

            model.EditorModel = book;
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
            if (model.EditorModel.Flight == null ||
                model.EditorModel.Passengers == null)
            {
                parent.ShowMessageAsync("Error", "Please fill all required fields");
                return;
            }

            model.IsEditorEnabled = false;

            string url = Url.BOOKINGS;
            Method method = Method.POST;

            if (model.EditorModel.Id != 0)
            {
                url += "/" + model.EditorModel.Id;
                method = Method.PUT;
            }

            BookModel bookModel = new BookModel
            {
                FlightId = model.EditorModel.Flight.Id,
                PassengerIds = model.EditorModel.Passengers.Select(p => p.Id).ToList()
            };

            ApiRequest request = new ApiRequest(url, method);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(bookModel);

            App.Client.ExecuteAsync<Book>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        parent.ShowMessageAsync("Error", "Unable to save booking information.");
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

        private void EditorDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            string message = "Are you sure you want to cancel this booking? This action is irreversible.";

            parent.ShowMessageAsync("Delete Booking", message, MessageDialogStyle.AffirmativeAndNegative).ContinueWith(task =>
            {
                if (task.Result == MessageDialogResult.Negative)
                {
                    return;
                }

                this.Dispatcher.Invoke(new Action(() => model.IsEditorEnabled = false));

                ApiRequest request = new ApiRequest(Url.BOOKINGS + "/" + model.EditorModel.Id, Method.DELETE);

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

        #endregion
    }
}
