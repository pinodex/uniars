using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Uniars.Shared.Database.Entity;

namespace Uniars.Client.UI.Pages.Main
{
    public class BookingModel : INotifyPropertyChanged
    {
        public const string P_IS_LOADING_ACTIVE = "IsLoadingActive";

        public const string P_IS_EDITOR_ENABLED = "IsEditorEnabled";

        public const string P_IS_EDIT_MODE = "IsEditMode";

        public const string P_BOOKING_LIST = "BookingList";

        public const string P_LAST_UPDATE_TIME = "LastUpdateTime";

        public const string P_PAGES = "Pages";

        public const string P_CURRENT_PAGE = "CurrentPage";

        public const string P_EDITOR_MODEL = "EditorModel";

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isLoadingActive = false;

        private bool _isEditorEnabled = true;

        private bool _isEditMode = false;

        private BindingList<Book> _bookingList = new BindingList<Book>();

        private DateTime _lastUpdateTime = DateTime.Now;

        private BindingList<int> _pages = new BindingList<int>();

        private int _currentPage = 1;

        private Book _editorModel;

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

        public BindingList<Book> BookingList
        {
            get
            {
                return _bookingList;
            }

            set
            {
                _bookingList = value;

                OnPropertyChanged(P_BOOKING_LIST);
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

        public Book EditorModel
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