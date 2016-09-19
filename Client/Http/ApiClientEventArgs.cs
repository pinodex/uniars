using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;

namespace Uniars.Client.Http
{
    public class ApiClientEventArgs : EventArgs
    {
        public IRestResponse Response { get; set; }
    }
}
