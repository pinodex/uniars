using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;

namespace Uniars.Server.Http.Module
{
    public abstract class BaseModule : NancyModule
    {
        protected int perPage = 100;

        public BaseModule(string modulePath)
            : base(modulePath)
        {
        }

        protected int GetCurrentPage()
        {
            try
            {
                return int.Parse(this.Request.Query["page"]);
            }
            catch { }

            return 1;
        }
    }
}
