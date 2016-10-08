using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Uniars.Client.UI
{
    public class LoginWindowModel : INotifyPropertyChanged
    {
        public const string P_SHOW_TOUCH_KEYBOARD = "ShowTouchKeyboard";

        private bool _showTouchKeyboard = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ShowTouchKeyboard
        {
            get
            {
                return _showTouchKeyboard;
            }

            set
            {
                _showTouchKeyboard = value;

                OnPropertyChanged(P_SHOW_TOUCH_KEYBOARD);
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
