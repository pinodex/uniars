using System.Windows;
using System.Windows.Controls;
using Uniars.Client.UI.Pages.Main;
using Uniars.Shared.Database.Entity;

namespace Uniars.Client.UI.Pages.Flyout
{
    /// <summary>
    /// Interaction logic for Passenger.xaml
    /// </summary>
    public partial class UserView : Page
    {
        private Users parent;

        public UserView(Users parent, User model)
        {
            InitializeComponent();

            this.DataContext = model;
            this.parent = parent;
        }

        #region Events

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            this.parent.parent.CloseFlyout();

            this.parent.model.IsEditMode = true;
            this.parent.model.EditorModel = this.DataContext as User;

            this.parent.SetActiveTab(1);
        }

        #endregion
    }
}
