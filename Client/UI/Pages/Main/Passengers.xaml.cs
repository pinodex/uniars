using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using RestSharp;
using Uniars.Client.Core.Collections;
using Uniars.Client.Http;
using Uniars.Client.UI.Pages.Flyout;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;

namespace Uniars.Client.UI.Pages.Main
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Passengers : Page
    {
        public MainWindow parent;

        public PassengersModel model = new PassengersModel();

        public bool deferAutoRefresh = false;

        public Passengers(MainWindow parentWindow)
        {
            InitializeComponent();

            this.DataContext = model;
            this.parent = parentWindow;

            model.PassengerList.ListChanged += (sender, e) =>
            {
                model.LastUpdateTime = DateTime.Now;
            };

            model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == PassengersModel.P_CURRENT_PAGE && !model.IsLoadingActive)
                {
                    this.LoadList();
                }
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

        /// <summary>
        /// Create blank Passenger model
        /// </summary>
        /// <returns></returns>
        public Passenger CreateBlankModel()
        {
            return new Passenger
            {
                Contacts = new List<PassengerContact>
                {
                    new PassengerContact()
                }
            };
        }

        /// <summary>
        /// Load list from server
        /// </summary>
        /// <param name="autoTriggered"></param>
        public void LoadList(bool autoTriggered = false)
        {
            model.IsLoadingActive = true && !autoTriggered;

            ApiRequest request = new ApiRequest(Url.PASSENGERS);
            request.AddParameter("page", model.CurrentPage);

            App.Client.ExecuteAsync<PaginatedResult<Passenger>>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                PaginatedResult<Passenger> result = response.Data;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    model.PassengerList.Repopulate<Passenger>(result.Data);

                    if (model.Pages.Count() == 0)
                    {
                        model.Pages.Repopulate<int>(result.GetPageList());
                    }

                    model.IsLoadingActive = false;

                    if (model.CurrentPage != result.Info.CurrentPage)
                    {
                        this.LoadList();
                    }
                }));
            });
        }

        /// <summary>
        /// Load countries from server
        /// </summary>
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

        /// <summary>
        /// Set active tab
        /// </summary>
        /// <param name="index">Tab index</param>
        public void SetActiveTab(int index)
        {
            tabs.SelectedIndex = index;
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
        /// Open flyout for Passenger
        /// </summary>
        /// <param name="model"></param>
        public void OpenFlyout(Passenger model)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                parent.SetFlyoutContent("Passenger", new PassengerView(this, model));
                parent.OpenFlyout();
            }));
        }

        #region Events

        private void ListRowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = ItemsControl.ContainerFromElement(
                sender as DataGrid, e.OriginalSource as DependencyObject) as DataGridRow;

            if (row == null)
            {
                return;
            }

            this.OpenFlyout(this.passengersTable.SelectedItem as Passenger);

            e.Handled = true;
        }

        private void ListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.OpenFlyout(this.passengersTable.SelectedItem as Passenger);

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
                        this.SearchCodeButtonClicked(sender, e);
                        break;

                    case "NameSearch":
                        this.SearchNameButtonClicked(sender, e);
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
                        this.ClearSearchButtonClicked(sender, e);

                        break;
                }
            }
        }

        private void SearchCodeButtonClicked(object sender, RoutedEventArgs e)
        {
            string code = searchCodeText.Text;

            if (code == string.Empty)
            {
                parent.ShowMessageAsync("Error", "Code cannot be empty.");
                return;
            }

            ApiRequest.ExecuteParams<Passenger>(Url.PASSENGERS + "/" + code, null, passenger =>
            {
                this.OpenFlyout(passenger);

                this.Dispatcher.Invoke(new Action(() =>
                {
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
                parent.ShowMessageAsync("Error", "Please fill at least one search criteria.");
                return;
            }

            model.IsLoadingActive = true;
            this.deferAutoRefresh = true;

            Dictionary<string, string> query = new Dictionary<string, string>
            {
                {"given_name", givenName},
                {"family_name", familyName},
                {"middle_name", middleName}
            };

            ApiRequest.ExecuteParams<PaginatedResult<Passenger>>(Url.PASSENGER_SEARCH, query, result => this.Dispatcher.Invoke(new Action(() =>
                {
                    model.PassengerList.Repopulate<Passenger>(result.Data);
                    
                    model.IsLoadingActive = false;
                }))
            );
        }

        private void EditorDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            string message = string.Format("Are you sure you want to delete \"{0}\" from passenger list? This action is irreversible.",
                model.EditorModel.DisplayName);

            parent.ShowMessageAsync("Delete Passenger", message, MessageDialogStyle.AffirmativeAndNegative).ContinueWith(task =>
            {
                if (task.Result == MessageDialogResult.Negative)
                {
                    return;
                }

                this.Dispatcher.Invoke(new Action(() => model.IsEditorEnabled = false));

                ApiRequest request = new ApiRequest(Url.PASSENGERS + "/" + model.EditorModel.Id, Method.DELETE);
                
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

        private void ClearSearchButtonClicked(object sender, RoutedEventArgs e)
        {
            searchGivenNameText.Text = string.Empty;
            searchFamilyNameText.Text = string.Empty;
            searchMiddleNameText.Text = string.Empty;

            this.deferAutoRefresh = false;
            this.LoadList();
        }

        private void SavePassengerButtonClicked(object sender, RoutedEventArgs e)
        {
            model.IsEditorEnabled = false;

            string url = Url.PASSENGERS;
            Method method = Method.POST;

            if (model.EditorModel.Id != 0)
            {
                url += "/" + model.EditorModel.Id;
                method = Method.PUT;
            }

            ApiRequest request = new ApiRequest(url, method);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(model.EditorModel);

            App.Client.ExecuteAsync<Passenger>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    this.Dispatcher.Invoke(new Action(() => parent.ShowMessageAsync("Error", "Unable to save passenger information.")));

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

        private void EditorClearButtonClicked(object sender, RoutedEventArgs e)
        {
            model.EditorModel = this.CreateBlankModel();
        }

        private void EditorDiscardButtonClicked(object sender, RoutedEventArgs e)
        {
            this.ResetEditor();
        }

        #endregion
    }
}
