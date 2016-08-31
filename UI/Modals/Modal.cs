using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Uniars.UI.Modals
{
    public class Modal : Window
    {
        public delegate void Callback(ModalEvent e);

        public event Callback SuccessCallback;
    }
}
