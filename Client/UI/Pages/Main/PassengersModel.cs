using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Uniars.Shared.Database.Entity;

namespace Uniars.Client.UI.Pages.Main
{
    public class PassengersModel : INotifyPropertyChanged
    {
        public const string P_IS_LOADING_ACTIVE = "IsLoadingActive";

        public const string P_IS_CODE_SEARCH_ENABLED = "IsCodeSearchEnabled";

        public const string P_IS_NAME_SEARCH_ENABLED = "IsNameSearchEnabled";

        public const string P_PASSENGER_LIST = "PassengerList";

        public const string P_LAST_UPDATE_TIME = "LastUpdateTime";

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isLoadingActive = false;

        private bool _isCodeSearchEnabled = true;

        private bool _isNameSearchEnabled = true;

        private BindingList<Passenger> _passengerList = new BindingList<Passenger>();

        private DateTime _lastUpdateTime = DateTime.Now;

        public bool IsLoadingActive
        {
            get
            {
                return _isLoadingActive;
            }

            set
            {
                _isLoadingActive = value;

                OnPropertyChanged(P_IS_LOADING_ACTIVE);
            }
        }

        public bool IsCodeSearchEnabled
        {
            get
            {
                return _isCodeSearchEnabled;
            }

            set
            {
                _isCodeSearchEnabled = value;

                OnPropertyChanged(P_IS_CODE_SEARCH_ENABLED);
            }
        }

        public bool IsNameSearchEnabled
        {
            get
            {
                return _isNameSearchEnabled;
            }

            set
            {
                _isNameSearchEnabled = value;

                OnPropertyChanged(P_IS_NAME_SEARCH_ENABLED);
            }
        }

        public BindingList<Passenger> PassengerList
        {
            get
            {
                return _passengerList;
            }

            set
            {
                _passengerList = value;

                OnPropertyChanged(P_PASSENGER_LIST);
            }
        }

        public DateTime LastUpdateTime
        {
            get
            {
                return _lastUpdateTime;
            }

            set
            {
                _lastUpdateTime = value;

                OnPropertyChanged(P_LAST_UPDATE_TIME);
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
