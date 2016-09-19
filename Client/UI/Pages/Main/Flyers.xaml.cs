using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Uniars.Shared.Database.Entity;
using Uniars.Client.Http;
using RestSharp;
using System.Net;

namespace Uniars.Client.UI.Pages.Main
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Flyers : Page
    {
        public Flyers()
        {
            InitializeComponent();

            this.Loaded += PageLoaded;
        }

        public void PageLoaded(object sender, EventArgs e)
        {
            ApiRequest request = new ApiRequest("/flyers");

            App.Client.ExecuteAsync<List<Flyer>>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                List<Flyer> flyers = response.Data;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.flyerTable.ItemsSource = flyers;
                }));
            });
        }
    }
}
