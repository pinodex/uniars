using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Security;
using Uniars.Server.Http.Auth;


namespace Uniars.Server.Http.Module
{
    public class MainModule : NancyModule
    {
        public MainModule() : base("/")
        {
            Get["/"] = Index;
            Get["/auth"] = Auth;
            Get["/health"] = Health;
        }

        public object Index(dynamic parameters)
        {
            return Response.AsText(string.Format("UNIARS-Server/{0}", App.Version), "text/plain");
        }

        public object Auth(dynamic parameters)
        {
            if (this.Context.CurrentUser != null)
            {
                return ((UserIdentity)this.Context.CurrentUser).User;
            }

            return HttpStatusCode.Unauthorized;
        }

        public object Health(dynamic parameters)
        {
            return HttpStatusCode.OK;
        }
    }
}
