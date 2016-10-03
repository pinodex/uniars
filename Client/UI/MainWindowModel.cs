using System;
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

        private DateTime _currentDateTime = DateTime.Now;

        private string _username = "Unknown";

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
