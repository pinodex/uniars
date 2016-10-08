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
    public class CountryModule : BaseModule
    {
        public CountryModule()
            : base("/countries")
        {
            this.RequiresAuthentication();

            Get["/"] = Index;
            Get["/all"] = All;
        }

        protected object Index(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<Country> db = context.Countries.OrderBy(Country => Country.Name);

                return new PaginatedResult<Country>(db, this.perPage, this.GetCurrentPage());
            }
        }

        protected object All(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                return context.Countries.OrderBy(Country => Country.Name).ToList();
            }
        }
    }
}
