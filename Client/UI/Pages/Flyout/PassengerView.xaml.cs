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
    public partial class PassengerView : Page
    {
        private Passengers parent;

        public PassengerView(Passengers parent, Passenger passenger)
        {
            InitializeComponent();

            this.DataContext = passenger;
            this.parent = parent;
        }

        public void EditPassengerButtonClick(object sender, RoutedEventArgs e)
        {
            this.parent.parent.CloseFlyout();

            this.parent.model.IsEditMode = true;
            this.parent.model.PassengerEditor = this.DataContext as Passenger;

            this.parent.SetActiveTab(1);
        }
    }
}
