﻿using System.Windows;
using System.Windows.Controls;
using Uniars.Client.UI.Pages.Main;
using Uniars.Shared.Database.Entity;

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

            controls.Visibility = parent.IsPickerEnabled() ? Visibility.Hidden : Visibility.Visible;
        }

        #region Events

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            this.parent.parent.CloseFlyout();

            this.parent.model.IsEditMode = true;
            this.parent.model.EditorModel = this.DataContext as Passenger;

            this.parent.SetActiveTab(1);
        }

        private void ViewBookingsButtonClick(object sender, RoutedEventArgs e)
        {
            this.parent.parent.CloseFlyout();
            this.parent.parent.SetOpenPage(2);

            this.parent.parent.GetPage<Booking>("Booking")
                .SearchByPassenger(this.DataContext as Passenger);
        }

        #endregion
    }
}
