using System;
using Nancy.Security;

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
