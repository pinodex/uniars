﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Uniars.Client.UI
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public const string P_CURRENT_DATE_TIME = "CurrentDateTime";

        public const string P_USERNAME = "Username";

        public const string P_SHOW_TOUCH_KEYBOARD = "ShowTouchKeyboard";

        private DateTime _currentDateTime = DateTime.Now;

        private string _username = "Unknown";

        private bool _showTouchKeyboard = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime CurrentDateTime
        {
            get
            {
                return _currentDateTime;
            }

            set
            {
                _currentDateTime = value;

                OnPropertyChanged(P_CURRENT_DATE_TIME);
            }
        }

        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value;

                OnPropertyChanged(P_USERNAME);
            }
        }

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
