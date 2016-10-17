using System.Data.Entity;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Uniars.Server.Http.Auth;
using Uniars.Server.Http.Response;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;

namespace Uniars.Server.Http.Module
{
    public class PassengerModule : BaseModule
    {
        public PassengerModule()
            : base("/passengers")
        {
            this.RequiresAuthentication();

            Get["/"] = Index;
            Get["/{id:int}"] = Single;
            Get["/{code}"] = SingleCode;

            Post["/"] = CreateModel;
            Put["/{id:int}"] = UpdateModel;
            Delete["/{id:int}"] = DeleteModel;
        }

        protected object Index(dynamic parameters)
        {
            string givenName = this.Request.Query["given_name"];
            string familyName = this.Request.Query["family_name"];
            string middleName = this.Request.Query["middle_name"];
            string displayName = this.Request.Query["display_name"];

            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<Passenger> db = context.Passengers
                    .Include(m => m.Contacts)
                    .Include(m => m.Contacts.Select(c => c.Country));

                if (givenName != null && givenName != string.Empty)
                {
                    db = db.Where(m => m.GivenName.Contains(givenName));
                }

                if (familyName != null && familyName != string.Empty)
                {
                    db = db.Where(m => m.FamilyName.Contains(familyName));
                }

                if (middleName != null && middleName != string.Empty)
                {
                    db = db.Where(m => m.MiddleName.Contains(middleName));
                }

                if (displayName != null && displayName != string.Empty)
                {
                    db = db.Where(m => m.DisplayName.Contains(displayName));
                }

                db = db.OrderBy(m => m.Id);

                return new PaginatedResult<Passenger>(db, this.perPage, this.GetCurrentPage());
            }
        }

        protected object Single(dynamic parameters)
        {
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Passenger model = context.Passengers
                    .Include(m => m.Contacts)
                    .Include(m => m.Contacts.Select(c => c.Country))
                    .FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger not found");
                }

                return model;
            }
        }

        protected object SingleCode(dynamic parameters)
        {
            string code = (string)parameters.code;

            using (Context context = new Context(App.ConnectionString))
            {
                Passenger model = context.Passengers
                    .Include(m => m.Contacts)
                    .Include(m => m.Contacts.Select(c => c.Country))
                    .FirstOrDefault(m => m.Code == code);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger not found");
                }

                return model;
            }
        }

        protected object CreateModel(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                Passenger model = this.Bind<Passenger>(
                    m => m.Id,
                    m => m.Code
                );

                model.GenerateCode();

                context.Passengers.Add(model);
                context.SaveChanges();

                return context.Passengers
                    .Include(m => m.Contacts)
                    .Include(m => m.Contacts.Select(c => c.Country))
                    .FirstOrDefault(m => m.Id == model.Id);
            }
        }

        protected object UpdateModel(dynamic parameters)
        {
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Passenger passenger = context.Passengers.FirstOrDefault(m => m.Id == id);

                if (passenger == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger not found");
                }

                this.BindTo(passenger,
                    m => m.Id,
                    m => m.Code,
                    m => m.CreatedAt,
                    m => m.UpdatedAt
                );

                context.SaveChanges();

                return context.Passengers
                    .Include(m => m.Contacts)
                    .Include(m => m.Contacts.Select(c => c.Country))
                    .FirstOrDefault(m => m.Id == id);
            }
        }

        protected object DeleteModel(dynamic parameters)
        {
            this.RequiresClaims(UserIdentity.ROLE_ADMIN);

            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Passenger model = context.Passengers.FirstOrDefault(m => m.Id == id);
                IQueryable<PassengerContact> contacts = context.PassengerContacts.Where(m => m.PassengerId == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger not found");
                }

                context.Passengers.Remove(model);
                context.PassengerContacts.RemoveRange(contacts);

                context.SaveChanges();
            }

            return HttpStatusCode.OK;
        }
    }
}
