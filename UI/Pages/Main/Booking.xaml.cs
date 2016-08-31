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
using Uniars.UI.Modals;

namespace Uniars.UI.Pages.Main
{
    /// <summary>
    /// Interaction logic for Booking.xaml
    /// </summary>
    public partial class Booking : Page
    {
        public Booking()
        {
            InitializeComponent();
        }

        protected void SetSource(ModalEvent e)
        {
            txtSource.Text = e.Value;
        }

        private void txtSource_GotFocus(object sender, EventArgs e)
        {
            Rect pos = Interface.GetAbsolutePlacement(txtSource, true);
            Modals.Booking.SelectSource selectSourceWindow = new Modals.Booking.SelectSource();

            selectSourceWindow.Left = pos.Left;
            selectSourceWindow.Top = pos.Top;

            selectSourceWindow.SuccessCallback += SetSource;
            selectSourceWindow.Show();
        }
    }
}
