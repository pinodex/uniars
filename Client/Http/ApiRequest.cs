using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;

namespace Uniars.Client.Http
{
    public class ApiRequest : RestRequest
    {
        public ApiRequest(string uri, Method method)
            : base(uri, method)
        {
        }
    }
}
