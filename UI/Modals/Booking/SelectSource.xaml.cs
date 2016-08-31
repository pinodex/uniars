﻿using System;
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
using System.Windows.Shapes;

namespace Uniars.UI.Modals.Booking
{
    /// <summary>
    /// Interaction logic for SelectSource.xaml
    /// </summary>
    public partial class SelectSource : Window
    {
        public event Modal.Callback SuccessCallback;

        public SelectSource()
        {
            InitializeComponent();
        }

        private void Window_LostFocus(object sender, EventArgs e)
        {
            Close();
        }
    }
}
