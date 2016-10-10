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
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : Page, IPollingList
    {
        public MainWindow parent;

        public UserModel model = new UserModel();

        public Dictionary<string, string> searchQuery;

        private bool disableAutoRefresh = false;

        private bool listLoadComplete = false;

        public Users(MainWindow parent)
        {
            InitializeComponent();

            this.DataContext = model;
            this.parent = parent;

            model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == UserModel.P_CURRENT_PAGE && !model.IsLoadingActive)
                {
                    this.LoadList();
                }
            };

            model.UserList.ListChanged += (sender, e) =>
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
        /// Create a blank User model
        /// </summary>
        /// <returns></returns>
        public User CreateBlankModel()
        {
            return new User();
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

            ApiRequest request = new ApiRequest(Url.USERS);
            request.AddParameter("page", model.CurrentPage);

            var responseAction = new Action<IRestResponse<PaginatedResult<User>>>(response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                PaginatedResult<User> result = response.Data;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    model.UserList.Repopulate<User>(result.Data);
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
                App.Client.ExecuteAsync<PaginatedResult<User>>(request, responseAction);
                return;
            }

            ApiRequest.ExecuteParams<PaginatedResult<User>>(request, this.searchQuery, responseAction);
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
        /// Open flyout for user
        /// </summary>
        /// <param name="model">User instance</param>
        public void OpenFlyout(User model)
        {
            if (model == null)
            {
                return;
            }

            model.Password = null;

            this.Dispatcher.Invoke(new Action(() =>
            {
                parent.SetFlyoutContent("User", new UserView(this, model));
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

            txtPassword.Password = string.Empty;
            txtPasswordConfirm.Password = string.Empty;

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
            searchNameText.Text = string.Empty;

            this.searchQuery = null;
            this.disableAutoRefresh = false;

            this.LoadList();
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            string name = searchNameText.Text;

            model.IsLoadingActive = true;
            this.disableAutoRefresh = true;

            this.searchQuery = new Dictionary<string, string>
            {
                {"name", name}
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

            this.OpenFlyout(this.table.SelectedItem as User);

            e.Handled = true;
        }

        private void ListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.OpenFlyout(this.table.SelectedItem as User);

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

            string url = Url.USERS;
            string password = txtPassword.Password;
            string passwordConfirm = txtPasswordConfirm.Password;

            if (password != passwordConfirm)
            {
                parent.ShowMessageAsync("Error", "Passwords do not match.");
                return;
            }

            Method method = Method.POST;

            if (model.EditorModel.Id != 0)
            {
                url += "/" + model.EditorModel.Id;
                method = Method.PUT;
            }

            if (password != string.Empty)
            {
                model.EditorModel.Password = password;
            }

            ApiRequest request = new ApiRequest(url, method);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(model.EditorModel);

            App.Client.ExecuteAsync<User>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        parent.ShowMessageAsync("Error", "Unable to save user information.");
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
            string message = string.Format("Are you sure you want to delete \"{0}\" from user list? This action is irreversible.",
                model.EditorModel.Name);

            parent.ShowMessageAsync("Delete User", message, MessageDialogStyle.AffirmativeAndNegative).ContinueWith(task =>
            {
                if (task.Result == MessageDialogResult.Negative)
                {
                    return;
                }

                this.Dispatcher.Invoke(new Action(() => model.IsEditorEnabled = false));

                ApiRequest request = new ApiRequest(Url.USERS + "/" + model.EditorModel.Id, Method.DELETE);

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
