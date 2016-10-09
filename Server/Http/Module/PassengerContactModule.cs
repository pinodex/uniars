using System.Data.Entity;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Uniars.Server.Http.Response;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;

namespace Uniars.Server.Http.Module
{
    public class PassengerContactModule : BaseModule
    {
        public PassengerContactModule()
            : base("/passengers/{passengerId}/contacts")
        {
            this.RequiresAuthentication();

            Get["/"] = Index;
            Get["/{id:int}"] = Single;

            Post["/"] = CreateModel;
            Put["/{id:int}"] = UpdateModel;
            Delete["/{id:int}"] = DeleteModel;
        }

        public object Index(dynamic parameters)
        {
            int passengerId = (int)parameters.passengerId;

            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<PassengerContact> db = context.PassengerContacts
                    .Where(m => m.PassengerId == passengerId) 
                    .OrderBy(m => m.Id);

                return db.ToList();
            }
        }

        protected object Single(dynamic parameters)
        {
            int passengerId = (int)parameters.passengerId;
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                PassengerContact model = context.PassengerContacts
                    .Where(m => m.Id == id)
                    .Where(m => m.PassengerId == passengerId)
                    .FirstOrDefault();

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger contact not found");
                }

                return model;
            }
        }

        protected object CreateModel(dynamic parameters)
        {
            int passengerId = (int)parameters.passengerId;

            using (Context context = new Context(App.ConnectionString))
            {
                Passenger passenger = context.Passengers
                    .Where(m => m.Id == passengerId)
                    .Include(m => m.Contacts)
                    .FirstOrDefault();

                if (passenger == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger not found");
                }

                PassengerContact model = this.Bind<PassengerContact>(
                    m => m.Id
                );

                passenger.Contacts.Add(model);
                context.SaveChanges();

                return model;
            }
        }

        protected object UpdateModel(dynamic parameters)
        {
            int passengerId = (int)parameters.passengerId;
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                PassengerContact model = context.PassengerContacts
                    .Where(m => m.Id == id)
                    .Where(m => m.PassengerId == passengerId)
                    .FirstOrDefault();

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger contact not found");
                }

                this.BindTo(model);

                context.SaveChanges();

                return model;
            }
        }

        protected object DeleteModel(dynamic parameters)
        {
            int passengerId = (int)parameters.passengerId;
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                PassengerContact model = context.PassengerContacts
                    .Where(m => m.Id == id)
                    .Where(m => m.PassengerId == passengerId)
                    .FirstOrDefault();

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger contact not found");
                }

                context.PassengerContacts.Remove(model);
                context.SaveChanges();
            }

            return HttpStatusCode.OK;
        }
    }
}
