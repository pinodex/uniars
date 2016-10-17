using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using RestSharp;
using Uniars.Client.Core;
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
    public partial class Passengers : Page, IPollingList, IPickable<Passenger>
    {
        public MainWindow parent;

        public PassengersModel model = new PassengersModel
        {
            CountryList = Commons.GetCountryList()
        };

        public Dictionary<string, string> searchQuery;

        private bool disableAutoRefresh = false;

        private bool listLoadComplete = false;

        public Passengers(MainWindow parentWindow)
        {
            InitializeComponent();

            this.DataContext = model;
            this.parent = parentWindow;

            model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == PassengersModel.P_CURRENT_PAGE && !model.IsLoadingActive)
                {
                    this.LoadList();
                }
            };

            model.PassengerList.ListChanged += (sender, e) =>
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
            if (autoTriggered && this.disableAutoRefresh)
            {
                return;
            }

            model.IsLoadingActive = true && !autoTriggered;

            ApiRequest request = new ApiRequest(Url.PASSENGERS);
            request.AddParameter("page", model.CurrentPage);

            var responseAction = new Action<IRestResponse<PaginatedResult<Passenger>>>(response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                PaginatedResult<Passenger> result = response.Data;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    model.PassengerList.Repopulate<Passenger>(result.Data);
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
                App.Client.ExecuteAsync<PaginatedResult<Passenger>>(request, responseAction);
                return;
            }

            ApiRequest.ExecuteParams<PaginatedResult<Passenger>>(request, this.searchQuery, responseAction);
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
            if (model == null)
            {
                return;
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                parent.SetFlyoutContent("Passenger", new PassengerView(this, model));
                parent.OpenFlyout();
            }));
        }

        public void EnablePicker(Action<List<Passenger>> result)
        {
            this.disableAutoRefresh = true;
            model.IsPickerMode = true;

            table.SelectionMode = DataGridSelectionMode.Extended;

            selectButton.Click += (sender, e) =>
            {
                result(table.SelectedItems.OfType<Passenger>().ToList());

                this.disableAutoRefresh = false;
                model.IsPickerMode = false;

                table.SelectionMode = DataGridSelectionMode.Single;
            };
        }

        public void EnablePicker(Action<Passenger> result)
        {
            this.disableAutoRefresh = true;
            model.IsPickerMode = true;

            selectButton.Click += (sender, e) =>
            {
                result(table.SelectedItem as Passenger);

                this.disableAutoRefresh = false;
                model.IsPickerMode = false;
            };
        }

        public bool IsPickerEnabled()
        {
            return model.IsPickerMode;
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

            this.OpenFlyout(this.table.SelectedItem as Passenger);

            e.Handled = true;
        }

        private void ListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.OpenFlyout(this.table.SelectedItem as Passenger);

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

            App.Client.ExecuteAsync<Passenger>(new ApiRequest(Url.PASSENGERS + "/" + code), response =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    searchCodeText.Text = string.Empty;

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        parent.ShowMessageAsync("Error", "Passenger with requested code cannot be found");
                        return;
                    }

                    this.OpenFlyout(response.Data);
                }));
            });
        }

        private void SearchNameButtonClicked(object sender, RoutedEventArgs e)
        {
            string displayName = searchDisplayNameText.Text;
            string givenName = searchGivenNameText.Text;
            string familyName = searchFamilyNameText.Text;
            string middleName = searchMiddleNameText.Text;

            if (displayName == string.Empty && givenName == string.Empty && familyName == string.Empty && middleName == string.Empty)
            {
                parent.ShowMessageAsync("Error", "Please fill at least one search criteria.");
                return;
            }

            model.IsLoadingActive = true;
            this.disableAutoRefresh = true;

            this.searchQuery = new Dictionary<string, string>
            {
                {"display_name", displayName},
                {"given_name", givenName},
                {"family_name", familyName},
                {"middle_name", middleName}
            };

            this.LoadList();
        }

        private void ClearSearchButtonClicked(object sender, RoutedEventArgs e)
        {
            searchGivenNameText.Text = string.Empty;
            searchFamilyNameText.Text = string.Empty;
            searchMiddleNameText.Text = string.Empty;

            this.searchQuery = null;
            this.disableAutoRefresh = false;

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

                    this.ResetEditor();
                }));
            });
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
