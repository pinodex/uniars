using System.Windows;
using System.Windows.Controls;
using Uniars.Client.UI.Pages.Main;
using Uniars.Shared.Database.Entity;

namespace Uniars.Client.UI.Pages.Flyout
{
    /// <summary>
    /// Interaction logic for FlightView.xaml
    /// </summary>
    public partial class FlightView : Page
    {
        private Flights parent;

        public FlightView(Flights parent, Flight model)
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
            this.parent.model.EditorModel = this.DataContext as Flight;

            this.parent.SetActiveTab(1);
        }

        #endregion
    }
}
