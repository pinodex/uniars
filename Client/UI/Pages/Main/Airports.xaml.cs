using System;
using System.Collections.Generic;
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
    /// Interaction logic for Airports.xaml
    /// </summary>
    public partial class Airports : Page, IPollingList, IPickable<Airport>
    {
        public MainWindow parent;

        public AirportsModel model = new AirportsModel
        {
            CountryList = Commons.GetCountryList()
        };

        public Dictionary<string, string> searchQuery;

        private bool disableAutoRefresh = false;

        private bool listLoadComplete = false;

        public Airports(MainWindow parent)
        {
            InitializeComponent();

            this.DataContext = model;
            this.parent = parent;

            model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == AirportsModel.P_CURRENT_PAGE && !model.IsLoadingActive)
                {
                    this.LoadList();
                }
            };

            model.AirportList.ListChanged += (sender, e) =>
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
        /// Create a blank Airport model
        /// </summary>
        /// <returns></returns>
        public Airport CreateBlankModel()
        {
            return new Airport();
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

            ApiRequest request = new ApiRequest(Url.AIRPORTS);
            request.AddParameter("page", model.CurrentPage);

            var responseAction = new Action<IRestResponse<PaginatedResult<Airport>>>(response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                PaginatedResult<Airport> result = response.Data;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    model.AirportList.Repopulate<Airport>(result.Data);
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
                App.Client.ExecuteAsync<PaginatedResult<Airport>>(request, responseAction);
                return;
            }

            ApiRequest.ExecuteParams<PaginatedResult<Airport>>(request, this.searchQuery, responseAction);
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
        /// Open flyout for airline
        /// </summary>
        /// <param name="model">Airport instance</param>
        public void OpenFlyout(Airport model)
        {
            if (model == null)
            {
                return;
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                parent.SetFlyoutContent("Airport", new AirportView(this, model));
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

        public void EnablePicker(Action<Airport> result)
        {
            this.disableAutoRefresh = true;
            model.IsPickerMode = true;

            selectButton.Click += (sender, e) =>
            {
                result(table.SelectedItem as Airport);

                this.disableAutoRefresh = false;
                model.IsPickerMode = false;
            };
        }

        public void EnablePicker(Action<List<Airport>> result)
        {
            throw new NotImplementedException();
        }

        public bool IsPickerEnabled()
        {
            return model.IsPickerMode;
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
            searchNameText.Text = string.Empty;
            searchCountryText.SelectedItem = null;

            this.searchQuery = null;
            this.disableAutoRefresh = false;

            this.LoadList();
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            string name = searchNameText.Text;
            string country = string.Empty;

            if (searchCountryText.SelectedItem != null)
            {
                country = (searchCountryText.SelectedItem as Country).Name;
            }

            model.IsLoadingActive = true;
            this.disableAutoRefresh = true;

            this.searchQuery = new Dictionary<string, string>
            {
                {"name", name},
                {"country", country}
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

            this.OpenFlyout(this.table.SelectedItem as Airport);

            e.Handled = true;
        }

        private void ListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.OpenFlyout(this.table.SelectedItem as Airport);

                e.Handled = true;
            }
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

            string url = Url.AIRPORTS;
            Method method = Method.POST;

            if (model.EditorModel.Id != 0)
            {
                url += "/" + model.EditorModel.Id;
                method = Method.PUT;
            }

            ApiRequest request = new ApiRequest(url, method);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(model.EditorModel);

            App.Client.ExecuteAsync<Airport>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        parent.ShowMessageAsync("Error", "Unable to save airline information.");
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
            string message = string.Format("Are you sure you want to delete \"{0}\" from airline list? This action is irreversible.",
                model.EditorModel.Name);

            parent.ShowMessageAsync("Delete Airport", message, MessageDialogStyle.AffirmativeAndNegative).ContinueWith(task =>
            {
                if (task.Result == MessageDialogResult.Negative)
                {
                    return;
                }

                this.Dispatcher.Invoke(new Action(() => model.IsEditorEnabled = false));

                ApiRequest request = new ApiRequest(Url.AIRPORTS + "/" + model.EditorModel.Id, Method.DELETE);

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
