using System;
using System.Windows.Controls;
using RestSharp;
using Uniars.Client.Http;
using Uniars.Shared.Model;
using System.Net;
using Uniars.Client.Core;

namespace Uniars.Client.UI.Pages.Main
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Overview : Page, IPollingList
    {
        private OverviewModel model = new OverviewModel();

        public Overview()
        {
            InitializeComponent();

            this.DataContext = model;

            int hourOfTheDay = DateTime.Now.Hour;
            string greeting = "Good Morning";

            if (hourOfTheDay > 12 && hourOfTheDay < 24)
            {
                greeting = "Good Evening";
            }

            model.Greeting = string.Format("{0}, {1}!", greeting, App.Client.CurrentUser.Name);

            this.Loaded += (sender, e) => this.LoadList();
        }

        public void LoadList(bool d = true)
        {
            ApiRequest request = new ApiRequest(Url.STATS);
            
            App.Client.ExecuteAsync<StatsModel>(request, result =>
            {
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    this.Dispatcher.Invoke(new Action(() => model.Stats = result.Data));
                }
            });
        }
    }
}
