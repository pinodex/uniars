using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Database.Entity;
using Nancy;
using Nancy.Security;
using Uniars.Server.Http.Response;
using Uniars.Shared.Database;

namespace Uniars.Server.Http.Module
{
    public class TimezoneModule : BaseModule
    {
        public TimezoneModule()
            : base("/timezones")
        {
            this.RequiresAuthentication();

            Get["/"] = Index;
        }

        protected object Index(dynamic parameters)
        {
            return TimeZoneInfo.GetSystemTimeZones();
        }
    }
}
