using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Uniars.Shared.Database.Entity;

namespace Uniars.Client.UI.Pages.Main
{
    public class AirlinesModel : INotifyPropertyChanged
    {
        public const string P_IS_LOADING_ACTIVE = "IsLoadingActive";

        public const string P_AIRLINE_LIST = "AirlineList";

        public const string P_COUNTRY_LIST = "CountryList";

        public const string P_LAST_UPDATE_TIME = "LastUpdateTime";

        public const string P_PAGES = "Pages";

        public const string P_CURRENT_PAGE = "CurrentPage";

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isLoadingActive = false;

        private BindingList<Airline> _airlineList = new BindingList<Airline>();

        private List<Country> _countryList = new List<Country>();

        private DateTime _lastUpdateTime = DateTime.Now;

        private BindingList<int> _pages = new BindingList<int>();

        private int _currentPage = 1;

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

        public BindingList<Airline> AirlineList
        {
            get
            {
                return _airlineList;
            }

            set
            {
                _airlineList = value;

                OnPropertyChanged(P_AIRLINE_LIST);
            }
        }

        public List<Country> CountryList
        {
            get
            {
                return _countryList;
            }

            set
            {
                _countryList = value;

                OnPropertyChanged(P_COUNTRY_LIST);
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

        public BindingList<int> Pages
        {
            get
            {
                return _pages;
            }

            set
            {
                _pages = value;

                OnPropertyChanged(P_PAGES);
            }
        }

        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }

            set
            {
                _currentPage = value;

                OnPropertyChanged(P_CURRENT_PAGE);
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