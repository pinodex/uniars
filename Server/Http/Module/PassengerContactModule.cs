using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using Uniars.Server.Http.Response;
using Uniars.Shared.Database.Entity;
using Uniars.Shared.Foundation;
using Uniars.Shared.Database;

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
                PassengerContact passengerContact = context.PassengerContacts
                    .Where(m => m.Id == id)
                    .Where(m => m.PassengerId == passengerId)
                    .FirstOrDefault();

                if (passengerContact == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger contact not found");
                }

                return passengerContact;
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

                PassengerContact passengerContact = this.Bind<PassengerContact>(
                    m => m.Id
                );

                passenger.Contacts.Add(passengerContact);
                context.SaveChanges();

                return passengerContact;
            }
        }

        protected object UpdateModel(dynamic parameters)
        {
            int passengerId = (int)parameters.passengerId;
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                PassengerContact passengerContact = context.PassengerContacts
                    .Where(m => m.Id == id)
                    .Where(m => m.PassengerId == passengerId)
                    .FirstOrDefault();

                if (passengerContact == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger contact not found");
                }

                this.BindTo(passengerContact);

                context.SaveChanges();

                return passengerContact;
            }
        }

        protected object DeleteModel(dynamic parameters)
        {
            int passengerId = (int)parameters.passengerId;
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                PassengerContact passengerContact = context.PassengerContacts
                    .Where(m => m.Id == id)
                    .Where(m => m.PassengerId == passengerId)
                    .FirstOrDefault();

                if (passengerContact == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger contact not found");
                }

                context.PassengerContacts.Remove(passengerContact);
                context.SaveChanges();
            }

            return HttpStatusCode.OK;
        }
    }
}
