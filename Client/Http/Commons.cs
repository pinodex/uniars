using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Database.Entity;
using System.Net;

namespace Uniars.Client.Http
{
    public class Commons
    {
        private static List<Country> countryList;

        public static List<Country> GetCountryList()
        {
            if (countryList != null)
            {
                return countryList;
            }

            ApiRequest request = new ApiRequest(Url.COUNTRIES_ALL);

            var response = App.Client.Execute<List<Country>>(request);

            countryList = response.Data;
            return countryList;
        }
    }
}
