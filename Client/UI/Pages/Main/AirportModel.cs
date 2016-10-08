﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Uniars.Shared.Database.Entity;

namespace Uniars.Client.UI.Pages.Main
{
    public class AirportsModel : INotifyPropertyChanged
    {
        public const string P_IS_LOADING_ACTIVE = "IsLoadingActive";

        public const string P_IS_EDITOR_ENABLED = "IsEditorEnabled";

        public const string P_IS_EDIT_MODE = "IsEditMode";

        public const string P_AIRPORT_LIST = "AirportList";

        public const string P_COUNTRY_LIST = "CountryList";

        public const string P_LAST_UPDATE_TIME = "LastUpdateTime";

        public const string P_PAGES = "Pages";

        public const string P_CURRENT_PAGE = "CurrentPage";

        public const string P_EDITOR_MODEL = "EditorModel";

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isLoadingActive = false;

        private bool _isEditorEnabled = true;

        private bool _isEditMode = false;

        private BindingList<Airport> _airportList = new BindingList<Airport>();

        private List<Country> _countryList = new List<Country>();

        private DateTime _lastUpdateTime = DateTime.Now;

        private BindingList<int> _pages = new BindingList<int>();

        private int _currentPage = 1;

        private Airport _editorModel;

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

        public bool IsEditorEnabled
        {
            get
            {
                return _isEditorEnabled;
            }

            set
            {
                _isEditorEnabled = value;

                OnPropertyChanged(P_IS_EDITOR_ENABLED);
            }
        }

        public bool IsEditMode
        {
            get
            {
                return _isEditMode;
            }

            set
            {
                _isEditMode = value;

                OnPropertyChanged(P_IS_EDIT_MODE);
            }
        }

        public BindingList<Airport> AirportList
        {
            get
            {
                return _airportList;
            }

            set
            {
                _airportList = value;

                OnPropertyChanged(P_AIRPORT_LIST);
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

        public Airport EditorModel
        {
            get
            {
                return _editorModel;
            }

            set
            {
                _editorModel = value;

                OnPropertyChanged(P_EDITOR_MODEL);
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