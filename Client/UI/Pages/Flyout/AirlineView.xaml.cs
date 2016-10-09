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
using Uniars.Shared.Database.Entity;
using Uniars.Client.UI.Pages.Main;

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
