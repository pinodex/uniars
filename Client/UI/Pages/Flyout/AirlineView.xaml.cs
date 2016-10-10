using System.Windows;
using System.Windows.Controls;
using Uniars.Client.UI.Pages.Main;
using Uniars.Shared.Database.Entity;

namespace Uniars.Client.UI.Pages.Flyout
{
    /// <summary>
    /// Interaction logic for Passenger.xaml
    /// </summary>
    public partial class AirlineView : Page
    {
        private Airlines parent;

        public AirlineView(Airlines parent, Airline model)
        {
            InitializeComponent();

            this.DataContext = model;
            this.parent = parent;

            controls.Visibility = parent.IsPickerEnabled() ? Visibility.Hidden : Visibility.Visible;
        }

        #region Events

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            this.parent.parent.CloseFlyout();

            this.parent.model.IsEditMode = true;
            this.parent.model.EditorModel = this.DataContext as Airline;

            this.parent.SetActiveTab(1);
        }

        #endregion
    }
}
