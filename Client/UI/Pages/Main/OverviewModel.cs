using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Uniars.Shared.Database.Entity;
using Uniars.Shared.Model;

namespace Uniars.Client.UI.Pages.Main
{
    public class OverviewModel : INotifyPropertyChanged
    {
        public const string P_GREETING = "Greeting";

        public const string P_STATS = "Stats";

        public event PropertyChangedEventHandler PropertyChanged;

        private string _greeting;

        private StatsModel _stats;

        public string Greeting
        {
            get
            {
                return _greeting;
            }

            set
            {
                _greeting = value;

                OnPropertyChanged(P_GREETING);
            }
        }

        public StatsModel Stats
        {
            get
            {
                return _stats;
            }

            set
            {
                _stats = value;

                OnPropertyChanged(P_STATS);
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