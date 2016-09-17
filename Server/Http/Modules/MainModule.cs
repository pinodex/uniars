using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Newtonsoft.Json;

namespace Uniars.Server.Http.Modules
{
    public class MainModule : NancyModule
    {
        public MainModule() : base("/")
        {
            Get["/"] = parameters =>
            {
                return Response.AsText(string.Format("UNIARS-Server/{0}", App.Version), "text/plain");
            };
        }
    }
}
