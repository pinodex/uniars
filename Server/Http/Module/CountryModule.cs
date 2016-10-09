using System.Linq;
using Nancy.Security;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;

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
